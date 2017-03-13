﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using Shared.Network;
using Shared.Objects;
using Shared.Util;

namespace LobbyServer.Network.Handlers
{
    public class Lobby
    {
        /*
         * byte 91 is level cap in short
        */

        public static byte[] GameSettings = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x06, 0x00, 0x2F, 0x37, 0x05, 0x00, 0x00, 0x00, 0x0F, 0x00,
            0xE4, 0xAF, 0x77, 0x00, 0xE8, 0x0F, 0x8B, 0x14, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xF3, 0x60, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x7F, 0x29, 0x20, 0x1C, 0x08, 0x87, 0x01, 0x01, 0x58, 0x02, 0x00, 0x00,
            0xFD, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x7B, 0x27, 0xE3, 0xCD, 0xF0, 0x8C, 0x02,
            0x0A, 0x5A, 0xAA, 0xD5, 0x29, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x05, 0xE0, 0x51, 0x00,
            0x41, 0x60, 0xC8, 0x00, 0x00, 0x08, 0x28, 0x02, 0x58, 0x11,
            0x83, 0x00, 0x64, 0x80,
            0xF8, 0xCA,
            0x01, 0x07, 0x30, 0x08, 0x00, 0x00, 0x00, 0x00, 0x3F
        };

        [Packet(Packets.CmdUserInfo)]
        public static void UserInfo(Packet packet)
        {
            var username = packet.Reader.ReadUnicodeStatic(32); // username
            var ticket = packet.Reader.ReadUInt32(); // Ticket

            Log.Debug("UserInfo request. (Username: {0}, Ticket: {1})", username, ticket);

            if (ticket != packet.Sender.User.Ticket)
            {
                Log.Error("Rejecting packet from {0} (ticket {1}) for invalid user-ticket combination.", username,
                    ticket);
                packet.Sender.Error("Invalid ticket-user combination.");
            }

            var ack = new Packet(Packets.GameSettingsAck);
            ack.Writer.Write(GameSettings);
            /*ack.Writer.Write(new byte[90]);
            ack.Writer.Write((short) 50); // Max Level?
            ack.Writer.Write(new byte[12]);*/
            packet.Sender.Send(ack);

            packet.Sender.User.Characters = CharacterModel.Retrieve(LobbyServer.Instance.Database.Connection,
                packet.Sender.User.UID);

            ack = new Packet(Packets.UserInfoAck);
            ack.Writer.Write(0); // Permissions
            ack.Writer.Write(packet.Sender.User.Characters.Count); // Characters count
            ack.Writer.WriteUnicodeStatic(packet.Sender.User.Name, 18); // Username

            ack.Writer.Write((long) 0);
            ack.Writer.Write((long) 0);
            ack.Writer.Write((long) 0);
            ack.Writer.Write(0);

            foreach (var character in packet.Sender.User.Characters)
            {
                Vehicle v = VehicleModel.Retrieve(LobbyServer.Instance.Database.Connection, character.CurrentCarId);
                var team = TeamModel.Retrieve(LobbyServer.Instance.Database.Connection, character.Tid);
                character.TeamId = team.TeamId;
                character.TeamName = team.TeamName;
                character.TeamMarkId = team.TeamMarkId;
                character.TeamCloseDate = (int)team.CloseDate;
                character.TeamRank = 1;

                ack.Writer.WriteUnicodeStatic(character.Name, 21); // Name
                ack.Writer.Write(character.Cid); // ID
                ack.Writer.Write((int)character.Avatar); // Avatar
                ack.Writer.Write((int)character.Level); // Level
                ack.Writer.Write(character.CurrentCarId); // CarID
                ack.Writer.Write(v.CarType); // CarType
                ack.Writer.Write(v.BaseColor); // CarColor
                ack.Writer.Write(character.CreationDate); // Creation Date
                ack.Writer.Write(character.Tid); // Crew ID
                ack.Writer.Write(character.TeamMarkId); // Crew Image
                ack.Writer.WriteUnicodeStatic("Staff", 13); // Crew Name
                ack.Writer.Write((short) 1); // Guild
                ack.Writer.Write((short) -1);
            }

            packet.Sender.Send(ack);
        }

        [Packet(Packets.CmdCheckInLobby)]
        public static void CheckInLobby(Packet packet)
        {
            uint version = packet.Reader.ReadUInt32();
            uint ticket = packet.Reader.ReadUInt32();
            string username = packet.Reader.ReadUnicodeStatic(0x28);
            uint time = packet.Reader.ReadUInt32();
            string stringTicket = packet.Reader.ReadAsciiStatic(0x40);

            User user = AccountModel.GetSession(LobbyServer.Instance.Database.Connection, username, ticket);
            if (user == null)
            {
                Log.Error("Rejecting {0} (ticket {1}) for invalid user-ticket combination.", username, ticket);
                packet.Sender.Error("Invalid ticket-user combination.");
                return;
            }
            packet.Sender.User = user;

            var ack = new Packet(Packets.CheckInLobbyAck); // CheckInLobbyAck
            ack.Writer.Write(0); // Result
            ack.Writer.Write(0); // Permission
            packet.Sender.Send(ack);

            var timeAck = new Packet(Packets.LobbyTimeAck); // LobbyTimeAck
            timeAck.Writer.Write(Environment.TickCount);
            timeAck.Writer.Write(Environment.TickCount);
            packet.Sender.Send(timeAck);

            Log.Debug("CheckInLobby {0} {1} {2} {3} {4}", version, ticket, username, time,
                BitConverter.ToString(Encoding.UTF8.GetBytes(stringTicket)));
        }

        [Packet(Packets.CmdCheckCharName)]
        public static void CheckCharacterName(Packet packet)
        {
            string characterName = packet.Reader.ReadUnicode();

            var ack = new Packet(Packets.CheckCharNameAck);
            ack.Writer.WriteUnicodeStatic(characterName, 21);
            ack.Writer.Write(CharacterModel.Exists(LobbyServer.Instance.Database.Connection, characterName)); // Availability. true = Available, false = Unavailable.
            packet.Sender.Send(ack);
        }

        /*
        000000: 41 00 64 00 6D 00 69 00 6E 00 00 00 00 01 00 00  A · d · m · i · n · · · · · · ·
        000016: 00 40 00 00 00 00 00 00 01 00 00 00 00 00 00 00  · @ · · · · · · · · · · · · · ·
        000032: 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00  · · · · · · · · · · · · · · · ·
        000048: 00 00 52 00 00 00 03 00 00 00  · · R · · · · · · ·

        000000: 41 00 64 00 6D 00 69 00 6E 00 69 00 73 00 74 00  A · d · m · i · n · i · s · t ·
        000016: 72 00 61 00 74 00 6F 00 72 00 00 00 00 00 00 00  r · a · t · o · r · · · · · · ·
        000032: 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00  · · · · · · · · · · · · · · · ·
        000048: 00 00 52 00 00 00 03 00 00 00  · · R · · · · · · ·
        */
        [Packet(Packets.CmdDeleteChar)]
        public static void DeleteCharacter(Packet packet)
        {
            //string charname = packet.Reader.ReadUnicode();
            string charname = packet.Reader.ReadUnicodeStatic(21);

            ulong charId = packet.Reader.ReadUInt64(); // Char ID?
            packet.Reader.ReadInt32(); // 82?
            packet.Reader.ReadInt32(); // 3?

            if (CharacterModel.HasCharacter(LobbyServer.Instance.Database.Connection, charId, packet.Sender.User.UID))
            {
                CharacterModel.DeleteCharacter(LobbyServer.Instance.Database.Connection, charId, packet.Sender.User.UID);
                var ack = new Packet(Packets.DeleteCharAck);
                ack.Writer.WriteUnicodeStatic(charname, 21);
                packet.Sender.Send(ack);

                return;
            }

            packet.Sender.Error("This character doesn't belong to you!");
        }
        [Packet(Packets.CmdCreateChar)]
        public static void CreateCharacter(Packet packet)
        {
           
            string characterName = packet.Reader.ReadUnicode();
            ulong UIDCreate = packet.Reader.ReadUInt64();
            ulong Avatar = 1;
          //  ulong Avatar = packet.Reader.ReadUInt64();
            CharacterModel.CreateCharacter(LobbyServer.Instance.Database.Connection, packet.Sender.User.UID, characterName, Avatar);
        

       

            // TODO: Verify, Handle

            var ack = new Packet(83);
            ack.Writer.WriteUnicodeStatic(characterName, 21);
            ack.Writer.Write(UIDCreate);
          //  ack.Writer.Write(Avatar);
            packet.Sender.Send(ack);
            packet.Sender.Error("Create Character Success Name: {0}", characterName);
        }
    }
}
