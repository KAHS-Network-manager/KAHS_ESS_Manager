-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2018-02-09 12:07:09.61

-- tables
-- Table: Message
CREATE TABLE Message (
    `Index` int NOT NULL AUTO_INCREMENT,
    Title varchar(20) NOT NULL,
    Content varchar(100) NOT NULL,
    Date timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT Message_pk PRIMARY KEY (`Index`)
);

-- Table: Student
CREATE TABLE Student (
    Number char(4) NOT NULL,
    Name varchar(5) NOT NULL,
    RoomNumber char(3) NOT NULL,
    Grade char(1) NOT NULL,
    Class char(1) NOT NULL,
    Ess char(1) NOT NULL DEFAULT 'N',
    Dormitory char(1) NOT NULL DEFAULT 'N',
    Outing char(1) NOT NULL DEFAULT 'N',
    OutingStart char(5) NULL,
    OutingEnd char(5) NULL,
    AcademyStart char(5) NULL,
    AcademyEnd char(5) NULL,
    Monday char(1) NULL,
    Tuesday char(1) NULL,
    Wednesday char(1) NULL,
    Thursday char(1) NULL,
    Friday char(1) NULL,
    Remarks varchar(50) NULL,
    CONSTRAINT Student_pk PRIMARY KEY (Number)
);

-- End of file.

