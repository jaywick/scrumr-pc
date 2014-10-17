DROP TABLE IF EXISTS Feature;
DROP TABLE IF EXISTS Project;
DROP TABLE IF EXISTS Sprint;
DROP TABLE IF EXISTS Ticket;
DROP TABLE IF EXISTS TicketType;

-- Table: Project
CREATE TABLE Project ( 
    ID                  INTEGER     PRIMARY KEY AUTOINCREMENT,
    Name                TEXT,
    NextProjectTicketId INTEGER     NOT NULL
                                    UNIQUE
);


-- Table: sprint
CREATE TABLE Sprint ( 
    ID        INTEGER      PRIMARY KEY AUTOINCREMENT,
    Name      TEXT,
    ProjectId INTEGER      REFERENCES Project ( ID ) 
);


-- Table: TicketType
CREATE TABLE TicketType ( 
    ID   INTEGER      PRIMARY KEY AUTOINCREMENT,
    Name TEXT  NOT NULL 
);

INSERT INTO [TicketType] ([ID], [Name]) VALUES (1, 'UserStory');
INSERT INTO [TicketType] ([ID], [Name]) VALUES (2, 'Bug');

-- Table: Feature
CREATE TABLE Feature ( 
    ID        INTEGER      PRIMARY KEY AUTOINCREMENT,
    Name      TEXT,
    ProjectId              REFERENCES Project ( ID ) 
);


-- Table: Ticket
CREATE TABLE Ticket ( 
    ID              INTEGER      PRIMARY KEY AUTOINCREMENT,
    Name            TEXT,
    Description     TEXT,
    FeatureId       INTEGER      NOT NULL
                                 REFERENCES feature ( ID ),
    SprintId        INTEGER      NOT NULL
                                 REFERENCES sprint ( ID ),
    ProjectTicketId INTEGER      NOT NULL
                                 UNIQUE,
    TypeId          INTEGER      NOT NULL
                                 REFERENCES TicketType ( ID ) 
);

