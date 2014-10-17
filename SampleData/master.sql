BEGIN TRANSACTION;

DROP TABLE IF EXISTS Features;
DROP TABLE IF EXISTS Projects;
DROP TABLE IF EXISTS Sprints;
DROP TABLE IF EXISTS Tickets;
DROP TABLE IF EXISTS TicketTypes;

CREATE TABLE "Tickets" (
	`ID`	INTEGER PRIMARY KEY AUTOINCREMENT,
	`Name`	TEXT,
	`Description`	TEXT,
	`FeatureId`	INTEGER NOT NULL,
	`SprintId`	INTEGER NOT NULL,
	`ProjectTicketId`	INTEGER NOT NULL,
	`TypeId`	INTEGER NOT NULL
);

CREATE TABLE "TicketTypes" (
	`ID` INTEGER PRIMARY KEY AUTOINCREMENT,
	`Name` TEXT NOT NULL
);

CREATE TABLE "Sprints" (
	`ID` INTEGER PRIMARY KEY AUTOINCREMENT,
	`Name` TEXT,
	`ProjectId` INTEGER REFERENCES [Project] (`ID`)
);

CREATE TABLE "Projects" (
	`ID`	INTEGER PRIMARY KEY AUTOINCREMENT,
	`Name`	TEXT,
	`NextProjectTicketId`	INTEGER NOT NULL
);

CREATE TABLE "Features" (
	`ID`	INTEGER PRIMARY KEY AUTOINCREMENT,
	`Name`	TEXT,
	`ProjectId`	INTEGER
);

INSERT INTO `Projects` VALUES(1,'Project 1',1);
INSERT INTO `Projects` VALUES(2,'Project X',2);

INSERT INTO `Features` VALUES(1,'Features 1',1);
INSERT INTO `Features` VALUES(2,'Features 2',1);
INSERT INTO `Features` VALUES(3,'Features 3',1);
INSERT INTO `Features` VALUES(4,'Feature X',2);

INSERT INTO `Sprints` VALUES(1,'Sprint 1',1);
INSERT INTO `Sprints` VALUES(2,'Sprint 2',1);
INSERT INTO `Sprints` VALUES(3,'Sprint 3',1);
INSERT INTO `Sprints` VALUES(4,'Sprint X',2);

INSERT INTO `TicketTypes` VALUES(1,'UserStory');
INSERT INTO `TicketTypes` VALUES(2,'Bug');

INSERT INTO `Tickets` VALUES(1,'Ticket 1','',1,1,1,1);
INSERT INTO `Tickets` VALUES(2,'Ticket 2','',1,2,2,1);
INSERT INTO `Tickets` VALUES(3,'Ticket 3','',1,1,3,1);
INSERT INTO `Tickets` VALUES(4,'Ticket 4','',2,2,4,1);
INSERT INTO `Tickets` VALUES(5,'Ticket 5','',2,2,5,1);
INSERT INTO `Tickets` VALUES(6,'Ticket 6','',1,3,6,1);
COMMIT;
