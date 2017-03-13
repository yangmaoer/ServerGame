﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;
using Shared.Network;
using Shared.Objects;
using Shared.Util;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace GameServer.Network.Handlers
{
    public class Game
    {
        /*
        000000: 2F ED 00 00  / · · · 
        000000: 56 B3 00 00  V · · ·

        Wrong Packet Size. CMD(3918) CmdLen: : 6, AnalysisSize: 4
        */
        [Packet(Packets.CmdUnknownSync)]
        public static void UnknownSync(Packet packet) // TODO: Figure out what this packet does...
        {
            packet.Reader.ReadInt32(); // always the same in session
            // hide sync packets for now

            var ack = new Packet(Packets.CmdUnknownSync+1);
            ack.Writer.Write((short)0);
        }

        [Packet(Packets.CmdMyCityPosition)] // TODO: Actual position and not just dummies
        public static void MyCityPosition(Packet packet)
        {
            //Console.WriteLine(packet.Reader.ReadUInt32());
            // -> Gate ID!
            var ack = new Packet(Packets.MyPositionAck);

            //ack.Writer.Write(256);
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.City); // City ID
            ack.Writer.Write(1); // Channel ID
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.PositionX); // x
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.PositionY); // y
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.PositionZ); // z
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.Rotation); // w
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.posState); // PosState
            /*int m_CityId;
            int m_ChannelId;
            XiVec4 m_Pos;
            int m_PositionState;*/

            packet.Sender.Send(ack);
        }


        /*
        000000: 00 00 00 00 00 00 00 00 00 00 00 00 4A 70 AB 42  · · · · · · · · · · · · J p · B
        000016: 00 00 00 00 01 00 00 00 01 00 00 00  · · · · · · · · · · · ·
        */
        [Packet(Packets.CmdSaveCarPos)]
        public static void SaveCarPos(Packet packet)
        {
            var channelId = packet.Reader.ReadInt32();
            var x = packet.Reader.ReadSingle();
            var y = packet.Reader.ReadSingle();
            var z = packet.Reader.ReadSingle();
            var w = packet.Reader.ReadSingle();
            var cityId = packet.Reader.ReadInt32();
            var posState = packet.Reader.ReadInt32();

            CharacterModel.UpdatePosition(GameServer.Instance.Database.Connection, packet.Sender.User.ActiveCharacterId, channelId, x, y, z, w, cityId, posState);
        }

        [Packet(Packets.CmdUpdateQuickSlot)]
        public static void UpdateQuickSlot(Packet packet)
        {
            // TODO: actually update the quickslots.
            var slot1 = packet.Reader.ReadUInt32();
            var slot2 = packet.Reader.ReadUInt32();
        }

        [Packet(Packets.CmdGetDateTime)]
        public static void GetDateTime(Packet packet)
        {
            var unixTimeNow = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            var now = DateTime.UtcNow.ToLocalTime();

            var globalTime = packet.Reader.ReadUInt32(); // GlobalTime
            var localTime = packet.Reader.ReadUInt32(); // LocalTime
            var action = packet.Reader.ReadUInt32(); // Action

            var ack = new Packet(Packets.GetDateTimeAck);
            ack.Writer.Write(action);
            ack.Writer.Write(globalTime);
            ack.Writer.Write(localTime);
            ack.Writer.Write((int)unixTimeNow.TotalSeconds);
            ack.Writer.Write(0); // ServerTickTime
            ack.Writer.Write(0); // ServerTick
            ack.Writer.Write((short)now.DayOfYear);
            ack.Writer.Write((short)now.Month);
            ack.Writer.Write((short)now.Day);
            ack.Writer.Write((short)now.DayOfWeek);
            ack.Writer.Write((byte)now.Hour);
            ack.Writer.Write((byte)now.Minute);
            ack.Writer.Write((byte)now.Second);

            packet.Sender.Send(ack);

            /*
              *(_DWORD *)(msg + 2) = lpMsg->Action;
              *(_DWORD *)(msg + 6) = lpMsg->GlobalTime;
              *(float *)(msg + 10) = lpMsg->LocalTime;
              *(_DWORD *)(msg + 14) = now;
              * *(_DWORD *)(msg + 18) = GetServerTickTime();
              *(_DWORD *)(msg + 22) = GetServerTick();
              *(_WORD *)(msg + 26) = ptm->tm_yday;
              *(_WORD *)(msg + 28) = ptm->tm_mon;
              *(_WORD *)(msg + 30) = ptm->tm_mday;
              *(_WORD *)(msg + 32) = ptm->tm_wday;
              *(_BYTE *)(msg + 34) = ptm->tm_hour;
              *(_BYTE *)(msg + 35) = ptm->tm_min;
              *(_BYTE *)(msg + 36) = ptm->tm_sec;
             */
        }

        

        /*
        000000: 01 00 00 00 01 00 00 00 00 00 00 00 00 00 80 3F  · · · · · · · · · · · · · · · ?
        000016: 00 00 00 00  · · · · 
        */
        [Packet(Packets.CmdFuelChargeReq)]
        public static void FuelChargeReq(Packet packet) // TODO: Send actual data
        {
            uint CarId = packet.Reader.ReadUInt32();
            long Pay = packet.Reader.ReadInt64();
            float fuel = packet.Reader.ReadSingle();

            /*
              unsigned int CarId;
              __unaligned __declspec(align(1)) __int64 Pay;
              float DeltaFuel;
              __int64 Gold;
              float Fuel;
              float UnitPrice;
              float SaleUnitPrice;
              float FuelCapacity;
              float FuelEfficiency;
              int SaleFlag;
            */

            var ack = new Packet(Packets.FuelChargeReqAck);
            ack.Writer.Write(CarId);
            ack.Writer.Write(Pay);
            ack.Writer.Write(fuel);
            ack.Writer.Write(packet.Sender.User.ActiveCharacter.MitoMoney);
            ack.Writer.Write(packet.Sender.User.ActiveCar.Mitron+fuel);
            ack.Writer.Write(20.0f); // Mito Price per liter
            ack.Writer.Write(15.0f); // Discounted Price per liter (Normal channel, major channel is free?)
            ack.Writer.Write(packet.Sender.User.ActiveCar.MitronCapacity);
            ack.Writer.Write(packet.Sender.User.ActiveCar.MitronEfficiency);

            ack.Writer.Write(0); // 1 = Item Discount 50%
            packet.Sender.Send(ack);
            /*
            SaleFlag = 0;
            if ( pGame->m_pCharInfo->m_bHalfMitronCharge )
            {
               fSaleRate = fSaleRate - 0.5;
                v7 = SaleFlag;
                LOBYTE(v7) = SaleFlag | 1;
                SaleFlag = v7;
            }
            */
        }

        [Packet(Packets.CmdChatMsg)]
        public static void ChatMessage(Packet packet)
        {
            string type = packet.Reader.ReadUnicodeStatic(10);
            bool green = packet.Reader.ReadUInt32() == 0xFF00FF00; // ignore this, use packet.Sender.Player.User.Status
            string message = packet.Reader.ReadUnicodePrefixed();

            string sender = packet.Sender.User.ActiveCharacter.Name;

            Log.Debug($"({type}) <{sender}> {message}");

            var ack = new Packet(Packets.ChatMsgAck);
            ack.Writer.WriteUnicodeStatic(type, 10);
            ack.Writer.WriteUnicodeStatic(sender, 18);
            ack.Writer.WriteUnicode(message);

            switch (type)
            {
                case "room":
                    GameServer.Instance.Server.Broadcast(ack); // TODO: broadcast only to users in same area
                    break;

                case "channel":
                    GameServer.Instance.Server.Broadcast(ack);
                    break;

                case "party":
                    GameServer.Instance.Server.Broadcast(ack); // TODO: broadcast only to users in same party
                    break;

                case "team":
                    GameServer.Instance.Server.Broadcast(ack); // TODO: broadcast only to users in same crew
                    break;

                default:
                    Log.Error("Undefined chat message type.");
                    break;
            }
        }

        [Packet(Packets.CmdMyTeamInfo)]
        public static void MyTeamInfo(Packet packet)
        {
            uint m_Act = packet.Reader.ReadUInt32(); // nAct?

            var ack = new Packet(Packets.MyTeamInfoAck);

            ack.Writer.Write(m_Act); // Act!?
            ack.Writer.Write(packet.Sender.User.ActiveCharacterId); // Char ID
            ack.Writer.Write(1); // Rank
            packet.Sender.User.ActiveTeam.Serialize(ack.Writer);
            ack.Writer.Write((ushort)0); // Age?
            packet.Sender.Send(ack);
            /*
              unsigned int m_Act;
              __int64 m_Cid;
              int m_TeamRank;
              XiStrTeamInfo m_TeamInfo;
              unsigned __int16 m_Age;
            */
        }

        [Packet(Packets.CmdGameCharInfo)]
        public static void GameCharInfo(Packet packet) // TODO: Send data correspoding to the charname, not user
        {
            /*
            [Debug] - GameCharInfo: 000000: 41 00 64 00 6D 00 69 00 6E 00 69 00 73 00 74 00  A · d · m · i · n · i · s · t ·
            000016: 72 00 61 00 74 00 6F 00 72 00 00 00 0A 00 06 00  r · a · t · o · r · · · · · · ·
            000032: 0D 25 33 65 63 65 69 76 65 64 00 00 00 00  · % 3 e c e i v e d · · · ·

            [Info] - Received GameCharInfo (id 660, 0x294) on 11021.

            Wrong Packet Size. CMD(661) CmdLen: : 1177, AnalysisSize: 831
            // We're missing 346 bytes of data.
            */
            var charName = packet.Reader.ReadUnicodeStatic(21);
            var ack = new Packet(Packets.GameCharInfoAck);
            packet.Sender.User.ActiveCharacter.Serialize(ack.Writer);
            packet.Sender.User.ActiveCar.Serialize(ack.Writer);
            var sinfo = new StatInfo();
            sinfo.Serialize(ack.Writer);
            packet.Sender.User.ActiveTeam.Serialize(ack.Writer);

            ack.Writer.Write((uint)0); // Serial
            ack.Writer.Write('A'); // LocType
            ack.Writer.Write('A'); // ChId
            ack.Writer.Write((ushort)1); // LocId

            packet.Sender.Send(ack);
        }

        [Packet(Packets.CmdChaseRequest)]
        public static void ChaseRequest(Packet packet)
        {
            /*
            [Debug] - ChaseRequest: 000000: 01 9A 45 91 45 66 4A AA 45 A6 52 E7 40 00 00 00  · · E · E f J · E · R · @ · · ·
            000016: 00  ·

            [Info] - Received ChaseRequest (id 189, 0xBD) on 11021.
            */

            var bNow = packet.Reader.ReadBoolean();
            var posX = packet.Reader.ReadSingle(); // X
            var posY = packet.Reader.ReadSingle(); // Y
            var posZ = packet.Reader.ReadSingle(); // Z
            var rot = packet.Reader.ReadSingle(); // W

            //Wrong Packet Size. CMD(186) CmdLen: : 252, AnalysisSize: 250
            var ack = new Packet(Packets.ChasePropose);
            ack.Writer.Write((ushort)0);
            ack.Writer.Write(posX); // Start X
            ack.Writer.Write(posY); // Start Y
            ack.Writer.Write(posZ); // Start Z
            ack.Writer.Write(rot); // Start W

            ack.Writer.Write(posX); // End X
            ack.Writer.Write(posY); // End Y
            ack.Writer.Write(posZ); // End Z

            ack.Writer.Write(0); // CourseId
            ack.Writer.Write(2 - (bNow ? 1 : 0)); // Type?
            ack.Writer.WriteUnicodeStatic("test", 100);

            ack.Writer.Write(1); // HUV first level
            ack.Writer.Write(10001); // HUV first Id
            packet.Sender.Send(ack);
        }

        /*
        [Packet(3200)]
        public static void ChangeArea(Packet packet)
        {
        }*/

        [Packet(Packets.CmdBuyItem)]
        public static void BuyItem(Packet packet)
        {

            int id = packet.Reader.ReadInt32();
            int quantity = packet.Reader.ReadInt32();
       
            MySqlCommand command = new MySqlCommand("SELECT Price FROM shop WHERE ItemID = @idd", GameServer.Instance.Database.Connection);
            command.Parameters.AddWithValue("@idd", id);

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {

                    var ack = new Packet(406);
                    ack.Writer.Write(id);
                    ack.Writer.Write(quantity);
                    ack.Writer.Write(reader.GetInt32(0) * quantity);


                    packet.Sender.Send(ack);



                }
                else
                {
                    packet.Sender.Error("Add item id = {0} to the database!", id);
                }

            }


        }
    }
}
