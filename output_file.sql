-- MySQL dump 10.13  Distrib 9.2.0, for macos13.7 (x86_64)
--
-- Host: localhost    Database: WorkFlowEngine
-- ------------------------------------------------------
-- Server version	9.2.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `Attechment`
--

DROP TABLE IF EXISTS `Attechment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Attechment` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `Type` varchar(100) DEFAULT NULL,
  `Ref_Obj` varchar(50) DEFAULT NULL,
  `Ref_ID` varchar(50) DEFAULT NULL,
  `Name` varchar(500) DEFAULT NULL,
  `Attechment` longblob,
  `Content_type` varchar(200) DEFAULT NULL,
  `Size` int DEFAULT NULL,
  `CreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CreateBy` varchar(100) DEFAULT NULL,
  `UpdateDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UpdateBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ID`),
  KEY `IX_Attechment` (`Ref_ID`,`Ref_Obj`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Attechment`
--

LOCK TABLES `Attechment` WRITE;
/*!40000 ALTER TABLE `Attechment` DISABLE KEYS */;
/*!40000 ALTER TABLE `Attechment` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DataBindValue`
--

DROP TABLE IF EXISTS `DataBindValue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DataBindValue` (
  `BindRowID` char(36) NOT NULL,
  `BindName` varchar(150) DEFAULT NULL,
  `Server` varchar(150) DEFAULT NULL,
  `SqlStatement` longtext,
  `DataFieldText` varchar(50) NOT NULL,
  `DataFieldValue` varchar(50) DEFAULT NULL,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`BindRowID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DataBindValue`
--

LOCK TABLES `DataBindValue` WRITE;
/*!40000 ALTER TABLE `DataBindValue` DISABLE KEYS */;
/*!40000 ALTER TABLE `DataBindValue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DeleteHistory`
--

DROP TABLE IF EXISTS `DeleteHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DeleteHistory` (
  `DeleteRowID` char(36) NOT NULL,
  `StartRowID` char(36) DEFAULT NULL,
  `WFRowID` char(36) DEFAULT NULL,
  `DocNo` varchar(100) DEFAULT NULL,
  `DeleteReason` longtext,
  `DeleteBy` varchar(50) DEFAULT NULL,
  `DeleteDate` datetime DEFAULT NULL,
  PRIMARY KEY (`DeleteRowID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DeleteHistory`
--

LOCK TABLES `DeleteHistory` WRITE;
/*!40000 ALTER TABLE `DeleteHistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `DeleteHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `DeletePermission`
--

DROP TABLE IF EXISTS `DeletePermission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `DeletePermission` (
  `WFRowID` char(36) DEFAULT NULL,
  `Username` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `DeletePermission`
--

LOCK TABLES `DeletePermission` WRITE;
/*!40000 ALTER TABLE `DeletePermission` DISABLE KEYS */;
/*!40000 ALTER TABLE `DeletePermission` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `GuestAccount`
--

DROP TABLE IF EXISTS `GuestAccount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `GuestAccount` (
  `UserName` varchar(50) NOT NULL,
  `Password` varchar(50) DEFAULT NULL,
  `Name` varchar(50) DEFAULT NULL,
  `Company` varchar(150) DEFAULT NULL,
  `Email` varchar(50) NOT NULL,
  `FullName` varchar(150) DEFAULT NULL,
  PRIMARY KEY (`UserName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `GuestAccount`
--

LOCK TABLES `GuestAccount` WRITE;
/*!40000 ALTER TABLE `GuestAccount` DISABLE KEYS */;
/*!40000 ALTER TABLE `GuestAccount` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `History_events`
--

DROP TABLE IF EXISTS `History_events`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `History_events` (
  `id` int NOT NULL AUTO_INCREMENT,
  `sql_statement` longtext,
  `date` datetime DEFAULT CURRENT_TIMESTAMP,
  `byuser` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `History_events`
--

LOCK TABLES `History_events` WRITE;
/*!40000 ALTER TABLE `History_events` DISABLE KEYS */;
/*!40000 ALTER TABLE `History_events` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `position_tmptb`
--

DROP TABLE IF EXISTS `position_tmptb`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `position_tmptb` (
  `PositionRowId` char(36) NOT NULL,
  `PositionName` varchar(150) NOT NULL,
  `WFRowId` char(36) NOT NULL,
  `PositionEmail` varchar(2550) DEFAULT NULL,
  `SelectedKey` varchar(150) DEFAULT NULL,
  `CrDate` datetime DEFAULT NULL,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT NULL,
  `UdBy` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `position_tmptb`
--

LOCK TABLES `position_tmptb` WRITE;
/*!40000 ALTER TABLE `position_tmptb` DISABLE KEYS */;
/*!40000 ALTER TABLE `position_tmptb` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Prosition`
--

DROP TABLE IF EXISTS `Prosition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `Prosition` (
  `PositionRowId` char(36) NOT NULL,
  `PositionName` varchar(150) NOT NULL,
  `WFRowId` char(36) NOT NULL,
  `PositionEmail` varchar(2550) DEFAULT NULL,
  `SelectedKey` varchar(150) DEFAULT NULL,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`PositionName`,`WFRowId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Prosition`
--

LOCK TABLES `Prosition` WRITE;
/*!40000 ALTER TABLE `Prosition` DISABLE KEYS */;
/*!40000 ALTER TABLE `Prosition` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `StepEnableObject`
--

DROP TABLE IF EXISTS `StepEnableObject`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `StepEnableObject` (
  `StepEnableObjectId` char(36) NOT NULL,
  `StepRowId` char(36) DEFAULT NULL,
  `TemplateRowId` char(36) DEFAULT NULL,
  `ObjectRowId` char(36) DEFAULT NULL,
  `BGColor` varchar(50) DEFAULT NULL,
  `Enable` char(10) DEFAULT NULL,
  `RequireField` char(1) DEFAULT 'N',
  `BindRowID` char(36) DEFAULT NULL,
  `DefaultVal` varchar(550) DEFAULT NULL,
  `Afteraction` varchar(1) DEFAULT 'N',
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`StepEnableObjectId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `StepEnableObject`
--

LOCK TABLES `StepEnableObject` WRITE;
/*!40000 ALTER TABLE `StepEnableObject` DISABLE KEYS */;
/*!40000 ALTER TABLE `StepEnableObject` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TemplateInputCurrentValue`
--

DROP TABLE IF EXISTS `TemplateInputCurrentValue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TemplateInputCurrentValue` (
  `WFRowID` char(36) NOT NULL,
  `StartRowID` char(36) NOT NULL,
  `ObjectName` varchar(250) NOT NULL,
  `Value` varchar(3000) DEFAULT NULL,
  `CrDate` datetime DEFAULT (cast(now() as date)),
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT (cast(now() as date)),
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`WFRowID`,`StartRowID`,`ObjectName`),
  KEY `IX_TemplateInputCurrentValue` (`WFRowID`),
  KEY `IX_TemplateInputCurrentValue_2` (`WFRowID`,`StartRowID`),
  KEY `IX_TemplateInputCurrentValue_3` (`WFRowID`,`StartRowID`,`ObjectName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TemplateInputCurrentValue`
--

LOCK TABLES `TemplateInputCurrentValue` WRITE;
/*!40000 ALTER TABLE `TemplateInputCurrentValue` DISABLE KEYS */;
/*!40000 ALTER TABLE `TemplateInputCurrentValue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `TemplateObject`
--

DROP TABLE IF EXISTS `TemplateObject`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `TemplateObject` (
  `ObjectRowId` char(36) NOT NULL,
  `TemplateRowId` char(36) NOT NULL,
  `ObjectName` varchar(50) NOT NULL,
  `ObjectType` varchar(50) DEFAULT NULL,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`TemplateRowId`,`ObjectName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `TemplateObject`
--

LOCK TABLES `TemplateObject` WRITE;
/*!40000 ALTER TABLE `TemplateObject` DISABLE KEYS */;
INSERT INTO `TemplateObject` VALUES ('d3063f92-a303-4eca-84e2-34622ddc47dd','7e8aa136-d1d7-40a2-8e37-783b9098ab67','lastname','TEXT','2025-03-30 19:07:10','admin','2025-03-30 19:07:10',NULL),('efd82148-7910-4fde-a60e-3ecf2659d6f5','7e8aa136-d1d7-40a2-8e37-783b9098ab67','name','TEXT','2025-03-30 19:07:10','admin','2025-03-30 19:07:10',NULL);
/*!40000 ALTER TABLE `TemplateObject` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tmp_position`
--

DROP TABLE IF EXISTS `tmp_position`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tmp_position` (
  `PositionRowId` char(36) NOT NULL,
  `PositionName` varchar(150) NOT NULL,
  `WFRowId` char(36) NOT NULL,
  `PositionEmail` varchar(2550) DEFAULT NULL,
  `SelectedKey` varchar(150) DEFAULT NULL,
  `CrDate` datetime DEFAULT NULL,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT NULL,
  `UdBy` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tmp_position`
--

LOCK TABLES `tmp_position` WRITE;
/*!40000 ALTER TABLE `tmp_position` DISABLE KEYS */;
/*!40000 ALTER TABLE `tmp_position` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(100) NOT NULL,
  `password` varchar(255) NOT NULL,
  `role` varchar(50) DEFAULT 'user',
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','yRWBrMuLXsiPUxvMIUZRRuQP/39nw5QzWMXHW1XumX0y.J0/MMbFa','admin');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WorkFlow`
--

DROP TABLE IF EXISTS `WorkFlow`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WorkFlow` (
  `WFRowId` char(36) NOT NULL,
  `WFName` varchar(550) DEFAULT NULL,
  `UserView` varchar(450) DEFAULT NULL,
  `UserEdit` varchar(450) DEFAULT NULL,
  `UserSpecialAction` varchar(2350) DEFAULT NULL,
  `SQLMailProfile` varchar(150) DEFAULT NULL,
  `ApplicationFolder` varchar(150) DEFAULT NULL,
  `SenderDisplay` varchar(150) DEFAULT NULL,
  `DefaultDomain` char(10) DEFAULT 'bst.co.th',
  `HisDisplayStdConfig` varchar(150) DEFAULT NULL,
  `HisDisplayConfigETC1` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC2` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC3` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC4` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC5` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC6` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC7` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC8` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC9` varchar(150) DEFAULT ';N;',
  `HisDisplayConfigETC10` varchar(150) DEFAULT ';N;',
  `FilterHistory` varchar(250) DEFAULT NULL,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`WFRowId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WorkFlow`
--

LOCK TABLES `WorkFlow` WRITE;
/*!40000 ALTER TABLE `WorkFlow` DISABLE KEYS */;
INSERT INTO `WorkFlow` VALUES ('a449482b-5777-471a-8d5e-bad359432f15','Booking meeting room','','admin','admin','','','','',NULL,';N;',';N;',';N;',';N;',';N;',';N;',';N;',';N;',';N;',';N;',NULL,'2025-03-30 10:41:00','1','2025-03-30 10:41:00',NULL);
/*!40000 ALTER TABLE `WorkFlow` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WorkFlowEventsHistory`
--

DROP TABLE IF EXISTS `WorkFlowEventsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WorkFlowEventsHistory` (
  `RowID` char(36) NOT NULL,
  `StartRowID` char(36) NOT NULL,
  `StepRowID` char(36) DEFAULT NULL,
  `Event` varchar(50) DEFAULT NULL,
  `ContentBody` longtext,
  `SentEmailTo` varchar(1500) DEFAULT NULL,
  `Email` varchar(1850) DEFAULT NULL,
  `CCEmail` varchar(1850) DEFAULT NULL,
  `EmailCommandForResent` longtext,
  `ExecuteCommand` longtext,
  `NextStep` varchar(550) DEFAULT NULL,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`RowID`),
  KEY `IX_WorkFlowEventsHistory_1` (`CrBy`),
  KEY `IX_WorkFlowEventsHistory_2` (`StepRowID`),
  KEY `IX_WorkFlowEventsHistory_3` (`StartRowID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WorkFlowEventsHistory`
--

LOCK TABLES `WorkFlowEventsHistory` WRITE;
/*!40000 ALTER TABLE `WorkFlowEventsHistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `WorkFlowEventsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WorkFlowRunningStatus`
--

DROP TABLE IF EXISTS `WorkFlowRunningStatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WorkFlowRunningStatus` (
  `StartRowID` char(36) NOT NULL,
  `WFRowId` char(36) DEFAULT NULL,
  `ParentStartRowID` char(36) DEFAULT NULL,
  `CurrentStepRowId` char(36) DEFAULT NULL,
  `ContentBody` longtext,
  `Status` varchar(50) DEFAULT 'Active',
  `ETC1` varchar(550) DEFAULT NULL,
  `ETC2` varchar(550) DEFAULT NULL,
  `ETC3` varchar(550) DEFAULT NULL,
  `ETC4` varchar(550) DEFAULT NULL,
  `ETC5` varchar(550) DEFAULT NULL,
  `ETC6` varchar(550) DEFAULT NULL,
  `ETC7` varchar(550) DEFAULT NULL,
  `ETC8` varchar(550) DEFAULT NULL,
  `ETC9` varchar(550) DEFAULT NULL,
  `ETC10` varchar(550) DEFAULT NULL,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`StartRowID`),
  KEY `IX_WorkFlowRunningStatus_1` (`ParentStartRowID`),
  KEY `IX_WorkFlowRunningStatus_2` (`WFRowId`),
  KEY `IX_WorkFlowRunningStatus_3` (`CurrentStepRowId`),
  KEY `IX_WorkFlowRunningStatus_4` (`CrDate`),
  KEY `IX_WorkFlowRunningStatus_5` (`Status`),
  KEY `IX_WorkFlowRunningStatus_6` (`StartRowID`,`WFRowId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WorkFlowRunningStatus`
--

LOCK TABLES `WorkFlowRunningStatus` WRITE;
/*!40000 ALTER TABLE `WorkFlowRunningStatus` DISABLE KEYS */;
/*!40000 ALTER TABLE `WorkFlowRunningStatus` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WorkFlowStep`
--

DROP TABLE IF EXISTS `WorkFlowStep`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WorkFlowStep` (
  `StepRowId` char(36) NOT NULL,
  `Seq` varchar(5) DEFAULT NULL,
  `GroupName` varchar(55) DEFAULT NULL,
  `WFRowId` char(36) NOT NULL,
  `StepName` varchar(250) NOT NULL,
  `TemplateRowId` char(36) DEFAULT NULL,
  `EnableSave` char(1) DEFAULT 'N',
  `AliasNameSave` varchar(150) DEFAULT NULL,
  `TriggerSave` text,
  `HideButtonSave` char(1) DEFAULT 'N',
  `CheckTiggerSave` char(1) DEFAULT 'N',
  `CheckConditionAutoStartSave` char(1) DEFAULT 'N',
  `EnableApprove` char(1) DEFAULT 'N',
  `AliasNameApprove` varchar(150) DEFAULT NULL,
  `TriggerApprove` text,
  `HideButtonApprove` char(1) DEFAULT 'N',
  `CheckTiggerApprove` char(1) DEFAULT 'N',
  `CheckConditionAutoStartApprove` char(1) DEFAULT 'N',
  `EnableReject` char(1) DEFAULT 'N',
  `AliasNameReject` varchar(150) DEFAULT NULL,
  `TriggerReject` text,
  `HideButtonReject` char(1) DEFAULT 'N',
  `CheckTiggerReject` char(1) DEFAULT 'N',
  `CheckConditionAutoStartReject` char(1) DEFAULT 'N',
  `DefaultStart` char(1) DEFAULT 'N',
  `StepAutoStart` varchar(15) DEFAULT NULL,
  `TiggerStartStepCondition` text,
  `TiggerStartStepConditionButton` varchar(10) DEFAULT NULL,
  `JavaScriptOnload` longtext,
  `EventBeforeSaveCallJavascriptFunction` text,
  `EventAfterSaveMailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledSaveValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterSaveCC1MailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledSaveCC1ValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterSaveInformMailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledSaveInformValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterSaveEmailSubject` text,
  `EventAfterSaveEmailContent` longtext,
  `EventAfterSaveInformEmailContent` longtext,
  `EventAfterSaveGotoStep` char(36) DEFAULT NULL,
  `DinamicFiledSaveValueToStep` text,
  `EventAfterSaveExecAtServer` varchar(50) DEFAULT NULL,
  `EventAfterSaveExecCommand` longtext,
  `EventAfterSaveUpdateValueETC1` text,
  `EventAfterSaveUpdateValueETC2` text,
  `EventAfterSaveUpdateValueETC3` text,
  `EventAfterSaveUpdateValueETC4` text,
  `EventAfterSaveUpdateValueETC5` text,
  `EventSaveDonotSendMail` varchar(1) DEFAULT 'N',
  `EventBeforeApproveCallJavascriptFunction` text,
  `EventAfterApproveMailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledApproveValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterApproveCC1MailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledApproveCC1ValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterApproveInformMailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledApproveInformValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterApproveEmailSubject` text,
  `EventAfterApproveEmailContent` longtext,
  `EventAfterApproveInformEmailContent` longtext,
  `EventAfterApproveGotoStep` char(36) DEFAULT NULL,
  `DinamicFiledApproveValueToStep` text,
  `EventAfterApproveExecAtServer` varchar(50) DEFAULT NULL,
  `EventAfterApproveExecCommand` longtext,
  `EventAfterApproveUpdateValueETC1` text,
  `EventAfterApproveUpdateValueETC2` text,
  `EventAfterApproveUpdateValueETC3` text,
  `EventAfterApproveUpdateValueETC4` text,
  `EventAfterApproveUpdateValueETC5` text,
  `EventApproveDonotSendMail` varchar(1) DEFAULT 'N',
  `EventBeforeRejectCallJavascriptFunction` text,
  `EventAfterRejectMailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledRejectValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterRejectCC1MailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledRejectCC1ValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterRejectInformMailToPosition` char(36) DEFAULT NULL,
  `DinamicFiledRejectInformValueToselect` varchar(150) DEFAULT NULL,
  `EventAfterRejectEmailSubject` text,
  `EventAfterRejectEmailContent` longtext,
  `EventAfterRejectInformEmailContent` longtext,
  `EventAfterRejectGotoStep` char(36) DEFAULT NULL,
  `DinamicFiledRejectValueToStep` text,
  `EventAfterRejectExecAtServer` varchar(50) DEFAULT NULL,
  `EventAfterRejectExecCommand` longtext,
  `EventAfterRejectUpdateValueETC1` text,
  `EventAfterRejectUpdateValueETC2` text,
  `EventAfterRejectUpdateValueETC3` text,
  `EventAfterRejectUpdateValueETC4` text,
  `EventAfterRejectUpdateValueETC5` text,
  `EventRejectDonotSendMail` varchar(1) DEFAULT 'N',
  `AllowUseUploadMgr` varchar(1) DEFAULT NULL,
  `AllowMoreUploadFile` varchar(1) DEFAULT NULL,
  `AllowDeleteFile` varchar(1) DEFAULT NULL,
  `AutoExpand` varchar(1) DEFAULT NULL,
  `Guideline` longtext,
  `DinamicKeyValue` text,
  `PassValueOldTemplate` varchar(1) DEFAULT 'N',
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`WFRowId`,`StepName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WorkFlowStep`
--

LOCK TABLES `WorkFlowStep` WRITE;
/*!40000 ALTER TABLE `WorkFlowStep` DISABLE KEYS */;
INSERT INTO `WorkFlowStep` VALUES ('e94aed18-58ee-4c60-a3cc-d80fd920dca8','1','','a449482b-5777-471a-8d5e-bad359432f15','request2',NULL,'1','','','0','N','N','0','','','0','N','N','0','','','0','N','N','1',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'N',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'N',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'N',NULL,NULL,NULL,NULL,NULL,'','N','2025-03-30 20:31:15',NULL,'2025-03-30 20:31:15',NULL);
/*!40000 ALTER TABLE `WorkFlowStep` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WorkFlowTemplate`
--

DROP TABLE IF EXISTS `WorkFlowTemplate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `WorkFlowTemplate` (
  `TemplateRowId` char(36) NOT NULL,
  `WFRowId` char(36) DEFAULT NULL,
  `TemplateName` varchar(550) DEFAULT NULL,
  `TemplateContent` longtext,
  `DesignData` longtext,
  `CrDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `CrBy` varchar(100) DEFAULT NULL,
  `UdDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `UdBy` varchar(100) DEFAULT NULL,
  `CssLib` varchar(100) DEFAULT NULL,
  `JsLib` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`TemplateRowId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WorkFlowTemplate`
--

LOCK TABLES `WorkFlowTemplate` WRITE;
/*!40000 ALTER TABLE `WorkFlowTemplate` DISABLE KEYS */;
INSERT INTO `WorkFlowTemplate` VALUES ('7e8aa136-d1d7-40a2-8e37-783b9098ab67','a449482b-5777-471a-8d5e-bad359432f15','F_booking','F_booking','{\"html\":\"<blockquote class=\\\"blockquote\\\"><div class=\\\"form-group mb-3\\\"><label for=\\\"Name\\\" class=\\\"form-label\\\"><table class=\\\"table table-bordered\\\" style=\\\"font-size: 1.25rem; color: var(--bs-body-color); font-family: var(--bs-body-font-family); font-weight: var(--bs-body-font-weight); text-align: var(--bs-body-text-align);\\\"><tbody><tr><td><h1 class=\\\"\\\" style=\\\"--tw-scale-x: 1; --tw-scale-y: 1; --tw-pan-x: ; --tw-pan-y: ; --tw-pinch-zoom: ; --tw-scroll-snap-strictness: proximity; --tw-gradient-from-position: ; --tw-gradient-via-position: ; --tw-gradient-to-position: ; --tw-ordinal: ; --tw-slashed-zero: ; --tw-numeric-figure: ; --tw-numeric-spacing: ; --tw-numeric-fraction: ; --tw-ring-inset: ; --tw-ring-offset-width: 0px; --tw-ring-offset-color: #fff; --tw-ring-color: rgb(59 130 246 \\/ 0.5); --tw-ring-offset-shadow: 0 0 #0000; --tw-ring-shadow: 0 0 #0000; --tw-shadow: 0 0 #0000; --tw-shadow-colored: 0 0 #0000; --tw-blur: ; --tw-brightness: ; --tw-contrast: ; --tw-grayscale: ; --tw-hue-rotate: ; --tw-invert: ; --tw-saturate: ; --tw-sepia: ; --tw-drop-shadow: ; --tw-backdrop-blur: ; --tw-backdrop-brightness: ; --tw-backdrop-contrast: ; --tw-backdrop-grayscale: ; --tw-backdrop-hue-rotate: ; --tw-backdrop-invert: ; --tw-backdrop-opacity: ; --tw-backdrop-saturate: ; --tw-backdrop-sepia: ; --tw-contain-size: ; --tw-contain-layout: ; --tw-contain-paint: ; --tw-contain-style: ; text-align: center;\\\"><span style=\\\"--tw-scale-x: 1; --tw-scale-y: 1; --tw-pan-x: ; --tw-pan-y: ; --tw-pinch-zoom: ; --tw-scroll-snap-strictness: proximity; --tw-gradient-from-position: ; --tw-gradient-via-position: ; --tw-gradient-to-position: ; --tw-ordinal: ; --tw-slashed-zero: ; --tw-numeric-figure: ; --tw-numeric-spacing: ; --tw-numeric-fraction: ; --tw-ring-inset: ; --tw-ring-offset-width: 0px; --tw-ring-offset-color: #fff; --tw-ring-color: rgb(59 130 246 \\/ 0.5); --tw-ring-offset-shadow: 0 0 #0000; --tw-ring-shadow: 0 0 #0000; --tw-shadow: 0 0 #0000; --tw-shadow-colored: 0 0 #0000; --tw-blur: ; --tw-brightness: ; --tw-contrast: ; --tw-grayscale: ; --tw-hue-rotate: ; --tw-invert: ; --tw-saturate: ; --tw-sepia: ; --tw-drop-shadow: ; --tw-backdrop-blur: ; --tw-backdrop-brightness: ; --tw-backdrop-contrast: ; --tw-backdrop-grayscale: ; --tw-backdrop-hue-rotate: ; --tw-backdrop-invert: ; --tw-backdrop-opacity: ; --tw-backdrop-saturate: ; --tw-backdrop-sepia: ; --tw-contain-size: ; --tw-contain-layout: ; --tw-contain-paint: ; --tw-contain-style: ; font-weight: bolder;\\\">\\u0e41\\u0e1a\\u0e1a\\u0e1f\\u0e2d\\u0e23\\u0e4c\\u0e21\\u0e02\\u0e2d\\u0e08\\u0e2d\\u0e07\\u0e2b\\u0e49\\u0e2d\\u0e07\\u0e1b\\u0e23\\u0e30\\u0e0a\\u0e38\\u0e21<\\/span><\\/h1><\\/td><\\/tr><\\/tbody><\\/table><\\/label><\\/div><\\/blockquote><div class=\\\"form-group mb-3\\\"><br><\\/div><table class=\\\"table table-bordered\\\"><tbody><tr><td><div class=\\\"form-group mb-3\\\"><label for=\\\"name\\\" class=\\\"form-label\\\">name<\\/label><input type=\\\"text\\\" class=\\\"form-control\\\" id=\\\"name\\\" name=\\\"name\\\" placeholder=\\\"name\\\"><\\/div><\\/td><td><div class=\\\"form-group mb-3\\\"><label for=\\\"lastname\\\" class=\\\"form-label\\\">last name<\\/label><input type=\\\"text\\\" class=\\\"form-control\\\" id=\\\"lastname\\\" name=\\\"lastname\\\" placeholder=\\\"last name\\\"><\\/div><\\/td><\\/tr><\\/tbody><\\/table><div class=\\\"form-group mb-3\\\"><br><\\/div>\",\"elements\":[{\"id\":\"name\",\"name\":\"name\",\"type\":\"input_text\"},{\"id\":\"lastname\",\"name\":\"lastname\",\"type\":\"input_text\"}]}','2025-03-30 11:00:51','admin','2025-03-30 19:07:10','admin',NULL,NULL);
/*!40000 ALTER TABLE `WorkFlowTemplate` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-03-30 21:41:49
