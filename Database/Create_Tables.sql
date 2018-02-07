-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2018-02-01 02:14:48.129

-- tables
-- Table: Academy
CREATE TABLE Academy (
    Number char(4) NOT NULL,
    Whether char(1) NOT NULL DEFAULT 'N',
    StartTime char(5) NULL,
    EndTime char(5) NULL,
    Monday char(1) NULL,
    Tuesday char(1) NULL,
    Wednesday char(1) NULL,
    Thursday char(1) NULL,
    Friday char(1) NULL,
    Remarks varchar(50) NULL,
    CONSTRAINT Academy_pk PRIMARY KEY (Number)
);

-- Table: ESS
CREATE TABLE ESS (
    Number char(4) NOT NULL,
    Ess char(1) NOT NULL DEFAULT 'N',
    Dormitory char(1) NOT NULL DEFAULT 'N',
    CONSTRAINT ESS_pk PRIMARY KEY (Number)
);

-- Table: Message
CREATE TABLE Message (
    `Index` int NOT NULL,
    Title varchar(20) NULL,
    Content varchar(100) NULL,
    Date date NULL DEFAULT NOW(),
    CONSTRAINT Message_pk PRIMARY KEY (`Index`)
);

-- Table: Outing
CREATE TABLE Outing (
    Number char(4) NOT NULL,
    Whether char(1) NOT NULL DEFAULT 'N',
    StartTime char(5) NULL,
    EndTime char(5) NULL,
    CONSTRAINT Outing_pk PRIMARY KEY (Number)
);

-- Table: Student
CREATE TABLE Student (
    Number char(4) NOT NULL,
    Name varchar(5) NOT NULL,
    RoomNumber char(3) NOT NULL,
    Grade char(1) NOT NULL,
    Class char(1) NOT NULL,
    CONSTRAINT Student_pk PRIMARY KEY (Number)
);

-- End of file.

