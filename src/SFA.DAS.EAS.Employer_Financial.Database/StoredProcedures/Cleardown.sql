﻿CREATE PROCEDURE [employer_financial].[Cleardown]
AS
	DELETE FROM [employer_financial].[EnglishFraction]
	DELETE FROM [employer_financial].[LevyDeclaration]
	DELETE FROM [employer_financial].[TopUpPercentage]
	DELETE FROM [employer_financial].[TransactionLine]
	DELETE FROM [employer_financial].[Payment]
	DELETE FROM [employer_financial].[PeriodEnd]
	DELETE FROM [employer_financial].[LevyDeclarationTopup]
	DELETE FROM [employer_financial].[EnglishFractionCalculationDate]
RETURN 0