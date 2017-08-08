CREATE PROCEDURE [Cleardown]
	@INCLUDETOPUPTABLE TINYINT = 0
AS
	DELETE FROM [EnglishFraction]
	DELETE FROM [LevyDeclaration]
	
	IF @INCLUDETOPUPTABLE = 0
	BEGIN
		DELETE FROM [TopUpPercentage]
	END
	DELETE FROM [LevyDeclarationTopup]
	DELETE FROM [EnglishFractionCalculationDate]
RETURN 0
