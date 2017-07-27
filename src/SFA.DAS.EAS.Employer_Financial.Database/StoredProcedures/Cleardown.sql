﻿CREATE PROCEDURE [employer_financial].[Cleardown]
	@INCLUDETOPUPTABLE TINYINT = 0
AS
	DELETE FROM [employer_financial].[EnglishFraction]
	DELETE FROM [employer_financial].[LevyDeclaration]
	
	IF @INCLUDETOPUPTABLE = 0
	BEGIN
		DELETE FROM [employer_financial].[TopUpPercentage]
	END
	DELETE FROM [employer_financial].[LevyDeclarationTopup]
	DELETE FROM [employer_financial].[EnglishFractionCalculationDate]
RETURN 0
