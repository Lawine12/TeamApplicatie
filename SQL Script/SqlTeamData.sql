CREATE TABLE [dbo].[TeamData]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [TeamName] VARBINARY(50) NOT NULL, 
    [TeamDriver] VARBINARY(50) NOT NULL, 
    [TeamCoach] VARBINARY(50) NOT NULL, 
    [Points] INT NULL
)
