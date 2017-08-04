CREATE TABLE [employer_levy].[EnglishFraction]
(
	[Id] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [DateCalculated] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 5) NULL, 
    [EmpRef] NVARCHAR(50) NULL
)

GO

CREATE INDEX [IX_EnglishFraction_Empref_DateCalculated] ON [employer_levy].[EnglishFraction] ([EmpRef], [DateCalculated]) WITH (ONLINE = ON)
