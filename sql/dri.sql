-- phpMyAdmin SQL Dump
-- version 4.6.5.2
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 13, 2017 at 08:55 AM
-- Server version: 10.1.21-MariaDB
-- PHP Version: 7.1.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `dri`
--

-- --------------------------------------------------------

--
-- Table structure for table `characters`
--

CREATE TABLE `characters` (
  `CID` bigint(20) NOT NULL,
  `UID` bigint(20) NOT NULL,
  `Name` varchar(21) NOT NULL,
  `Mito` bigint(20) NOT NULL DEFAULT '1000',
  `Avatar` int(11) NOT NULL DEFAULT '1',
  `Level` int(11) NOT NULL DEFAULT '1',
  `City` int(11) NOT NULL DEFAULT '1',
  `CurrentCarID` int(11) NOT NULL DEFAULT '1',
  `GarageLevel` int(11) NOT NULL DEFAULT '1',
  `TID` bigint(20) NOT NULL DEFAULT '0',
  `InventoryLevel` int(11) NOT NULL DEFAULT '1',
  `posX` double NOT NULL,
  `posY` double NOT NULL,
  `posZ` double NOT NULL,
  `posW` double NOT NULL,
  `posState` int(11) NOT NULL DEFAULT '1',
  `CreationDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `channelId` int(32) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `characters`
--

INSERT INTO `characters` (`CID`, `UID`, `Name`, `Mito`, `Avatar`, `Level`, `City`, `CurrentCarID`, `GarageLevel`, `TID`, `InventoryLevel`, `posX`, `posY`, `posZ`, `posW`, `posState`, `CreationDate`, `channelId`) VALUES
(1, 1, 'test', 5000000000000, 1, 55, 1, 1, 9, 0, 9, -3379, 1213.2, 85.51687, -0.0004243072, 3, '2017-03-01 05:32:28', 0),
(31, 1, 'sffujj', 1000, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 1, '2017-03-02 15:09:48', 0);

-- --------------------------------------------------------

--
-- Table structure for table `shop`
--

CREATE TABLE `shop` (
  `ItemID` bigint(20) NOT NULL,
  `Price` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `shop`
--

INSERT INTO `shop` (`ItemID`, `Price`) VALUES
(0, 250),
(5, 400),
(10, 1050),
(15, 2000),
(20, 3250),
(25, 250),
(30, 400),
(35, 1050),
(40, 2000),
(45, 3250),
(50, 250),
(55, 400),
(60, 1050),
(65, 2000),
(70, 3250),
(75, 250),
(80, 400),
(85, 1050),
(90, 2000),
(95, 3250),
(1445, 5000),
(1488, 50),
(1502, 200),
(1503, 1000),
(1504, 30000),
(1516, 5000),
(1554, 100),
(1561, 1000),
(1568, 500),
(1569, 1000),
(1570, 2000),
(1665, 1000),
(1666, 1000),
(1667, 1000),
(1818, 3000),
(1874, 49000),
(1875, 63000),
(1876, 98000),
(1877, 196000),
(2546, 4000),
(2547, 6000),
(2548, 7000),
(1979, 1200),
(1980, 700),
(1981, 2000),
(1982, 100),
(2032, 1000),
(1989, 10000),
(2013, 15000),
(2014, 15000),
(2015, 15000),
(2034, 10000),
(2068, 5000),
(2069, 7000),
(2070, 10000),
(2700, 10000),
(2708, 10000),
(2709, 10000),
(2003, 10000),
(2004, 12000),
(2005, 15000),
(2031, 12000),
(2036, 20000),
(2025, 30000),
(2214, 1200),
(2215, 700),
(2216, 2000),
(2127, 100),
(2177, 1000),
(2124, 1200),
(2125, 700),
(2126, 1000),
(2134, 10000),
(2158, 15000);

-- --------------------------------------------------------

--
-- Table structure for table `teams`
--

CREATE TABLE `teams` (
  `TID` int(65) NOT NULL,
  `TMARKID` int(65) NOT NULL,
  `TEAMNAME` varchar(655) COLLATE utf8_unicode_ci NOT NULL,
  `TEAMDESC` varchar(655) COLLATE utf8_unicode_ci NOT NULL,
  `TEAMURL` varchar(655) COLLATE utf8_unicode_ci NOT NULL,
  `CREATEDATE` int(65) NOT NULL,
  `CLOSEDATE` int(65) NOT NULL,
  `BANISHDATE` int(65) NOT NULL,
  `OWNCHANNEL` varchar(655) COLLATE utf8_unicode_ci NOT NULL,
  `TEAMSTATE` varchar(655) COLLATE utf8_unicode_ci NOT NULL,
  `TEAMRANKING` int(65) NOT NULL,
  `TEAMPOINT` int(65) NOT NULL,
  `CHANNELWINCNT` int(65) NOT NULL,
  `MEMBERCNT` int(65) NOT NULL,
  `CID` int(65) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- --------------------------------------------------------

--
-- Table structure for table `updates`
--

CREATE TABLE `updates` (
  `path` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `updates`
--

INSERT INTO `updates` (`path`) VALUES
('main.sql');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `UID` bigint(20) NOT NULL,
  `Username` varchar(21) NOT NULL,
  `Password` varchar(32) NOT NULL,
  `Status` tinyint(4) NOT NULL DEFAULT '1',
  `CreateIP` varchar(15) NOT NULL DEFAULT '127.0.0.1',
  `CreateDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `Ticket` varchar(655) NOT NULL,
  `Salt` varchar(65) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`UID`, `Username`, `Password`, `Status`, `CreateIP`, `CreateDate`, `Ticket`, `Salt`) VALUES
(1, 'admin', 'admin', 1, '127.0.0.1', '2017-03-02 15:18:18', '2478757461', 'Ex3lBoW1BmgG/piDti2MblqtPsec+t+Ruz3YMetHk/0='),
(2, 'test123', '9lps6TOgjzXPVP5t/90Anmqwt/Idml8d', 1, '127.0.0.1', '2017-02-07 11:35:37', '0', 'PyCovg5RPOlQWP+eLbps'),
(3, '123456', '3+ap+lfuTlooNxk7a45py2VqvZnEt25s', 1, '127.0.0.1', '2017-02-07 11:37:45', '0', 'BhIo2fqaN2nvAfXrvHu9');

-- --------------------------------------------------------

--
-- Table structure for table `vehicles`
--

CREATE TABLE `vehicles` (
  `CID` bigint(20) NOT NULL,
  `CharID` bigint(20) NOT NULL,
  `auctionCount` int(11) NOT NULL DEFAULT '0',
  `baseColor` int(11) NOT NULL DEFAULT '1',
  `carType` int(11) NOT NULL DEFAULT '1',
  `grade` int(11) NOT NULL DEFAULT '1',
  `mitron` double(11,2) NOT NULL DEFAULT '1000.00',
  `kmh` double(11,2) NOT NULL DEFAULT '1000.00',
  `slotType` int(11) NOT NULL DEFAULT '0',
  `color` int(11) NOT NULL DEFAULT '0',
  `mitronCapacity` double(11,2) NOT NULL DEFAULT '10.00',
  `mitronEfficiency` double(11,2) NOT NULL DEFAULT '20.00'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `vehicles`
--

INSERT INTO `vehicles` (`CID`, `CharID`, `auctionCount`, `baseColor`, `carType`, `grade`, `mitron`, `kmh`, `slotType`, `color`, `mitronCapacity`, `mitronEfficiency`) VALUES
(1, 1, 3, 1, 20, 10, 50.00, 3000.00, 1, 2, 300.00, 5000.00),
(31, 31, 1, 1, 1, 1, 1000.00, 1000.00, 1, 2, 10.00, 20.00);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `characters`
--
ALTER TABLE `characters`
  ADD PRIMARY KEY (`CID`),
  ADD KEY `UID` (`UID`);

--
-- Indexes for table `updates`
--
ALTER TABLE `updates`
  ADD PRIMARY KEY (`path`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`UID`);

--
-- Indexes for table `vehicles`
--
ALTER TABLE `vehicles`
  ADD PRIMARY KEY (`CID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `characters`
--
ALTER TABLE `characters`
  MODIFY `CID` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=32;
--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `UID` bigint(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;
--
-- Constraints for dumped tables
--

--
-- Constraints for table `characters`
--
ALTER TABLE `characters`
  ADD CONSTRAINT `characters_ibfk_1` FOREIGN KEY (`UID`) REFERENCES `users` (`UID`);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
