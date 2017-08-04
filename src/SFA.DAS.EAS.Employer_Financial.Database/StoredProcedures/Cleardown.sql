CREATE PROCEDURE [employer_levy].[Cleardown]
	@INCLUDETOPUPTABLE TINYINT = 0
AS
	DELETE FROM [employer_levy].[EnglishFraction]
	DELETE FROM [employer_levy].[LevyDeclaration]
	
	IF @INCLUDETOPUPTABLE = 0
	BEGIN
		DELETE FROM [employer_levy].[TopUpPercentage]
	END
	DELETE FROM [employer_levy].[LevyDeclarationTopup]
	DELETE FROM [employer_levy].[EnglishFractionCalculationDate]
RETURN 0
