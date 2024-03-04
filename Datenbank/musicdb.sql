-- --------------------------------------------------------
-- Host:                         127.0.0.2
-- Server-Version:               8.0.34 - MySQL Community Server - GPL
-- Server-Betriebssystem:        Win64
-- HeidiSQL Version:             12.5.0.6677
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Exportiere Datenbank-Struktur für musicdb
CREATE DATABASE IF NOT EXISTS `musicdb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `musicdb`;

-- Exportiere Struktur von Tabelle musicdb.album
CREATE TABLE IF NOT EXISTS `album` (
  `Album_ID` int NOT NULL AUTO_INCREMENT,
  `Artist_ID` int NOT NULL,
  `Album_Title` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `Year` int NOT NULL DEFAULT '0',
  `Label` varchar(50) NOT NULL,
  PRIMARY KEY (`Album_ID`),
  KEY `FK_alben_interpreten` (`Artist_ID`) USING BTREE,
  CONSTRAINT `FK_alben_interpreten` FOREIGN KEY (`Artist_ID`) REFERENCES `artists` (`Artist_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle musicdb.artists
CREATE TABLE IF NOT EXISTS `artists` (
  `Artist_ID` int NOT NULL AUTO_INCREMENT,
  `Artist_Name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `Year` int DEFAULT NULL,
  `Origin` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Genre` int NOT NULL,
  PRIMARY KEY (`Artist_ID`) USING BTREE,
  UNIQUE KEY `Interpret_Name` (`Artist_Name`) USING BTREE,
  KEY `FK_interpreten_genre` (`Genre`),
  CONSTRAINT `FK_interpreten_genre` FOREIGN KEY (`Genre`) REFERENCES `genre` (`Genre_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle musicdb.genre
CREATE TABLE IF NOT EXISTS `genre` (
  `Genre_ID` int NOT NULL AUTO_INCREMENT,
  `Genre_Name` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`Genre_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle musicdb.lyrics
CREATE TABLE IF NOT EXISTS `lyrics` (
  `Lyrics_ID` int NOT NULL AUTO_INCREMENT,
  `Song_ID` int NOT NULL,
  `Lyrics` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Lyrics_ID`) USING BTREE,
  KEY `FK_songtexte_songs` (`Song_ID`),
  CONSTRAINT `FK_songtexte_songs` FOREIGN KEY (`Song_ID`) REFERENCES `songs` (`Song_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle musicdb.members
CREATE TABLE IF NOT EXISTS `members` (
  `Member_ID` int NOT NULL AUTO_INCREMENT,
  `Artist_ID` int NOT NULL,
  `Name` varchar(50) NOT NULL DEFAULT '',
  `Rolle` varchar(50) NOT NULL DEFAULT '',
  PRIMARY KEY (`Member_ID`) USING BTREE,
  KEY `FK_mitglieder_interpreten` (`Artist_ID`) USING BTREE,
  CONSTRAINT `FK_mitglieder_interpreten` FOREIGN KEY (`Artist_ID`) REFERENCES `artists` (`Artist_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten-Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle musicdb.songs
CREATE TABLE IF NOT EXISTS `songs` (
  `Song_ID` int NOT NULL AUTO_INCREMENT,
  `Album_ID` int NOT NULL,
  `Song_Title` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `Track` int NOT NULL DEFAULT '0',
  `Lyrics` text,
  PRIMARY KEY (`Song_ID`) USING BTREE,
  KEY `FK_songs_alben` (`Album_ID`),
  KEY `Song_Titel` (`Song_Title`) USING BTREE,
  CONSTRAINT `FK_songs_alben` FOREIGN KEY (`Album_ID`) REFERENCES `album` (`Album_ID`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Daten-Export vom Benutzer nicht ausgewählt

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
