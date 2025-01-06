IF NOT EXISTS ( SELECT * FROM sys.databases WHERE name=N'TestDB')
BEGIN
	CREATE DATABASE TestDB
END
GO

USE TestDB

IF OBJECT_ID(N'Customers',N'U') IS NULL
BEGIN
	CREATE TABLE Customers (
		Id INT PRIMARY KEY IDENTITY(1,1),
		FirstName VARCHAR(100) NOT NULL,
		LastName VARCHAR(100) NOT NULL,
		Dob DATE NOT NULL
		)
END

Go

INSERT INTO Customers (FirstName, LastName, Dob)
VALUES('Mani','Maran','1 Jan 2000'),('John','Deo','5 Sep 1993')