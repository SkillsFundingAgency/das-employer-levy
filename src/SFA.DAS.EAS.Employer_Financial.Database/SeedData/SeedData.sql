/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

DECLARE @sqlCommand NVARCHAR(4000)
SELECT @sqlCommand = '
CREATE PROCEDURE #CreateLevy
	@payeScheme NVARCHAR(50),
	@levyAmount DECIMAL
AS
BEGIN
	DECLARE @submissionDate DATETIME
	SET @submissionDate = GETDATE()

	EXEC [employer_levy].[CreateDeclaration] @levyAmount, @payeScheme, @submissionDate, 123, 123, @levyAmount, ''17-18'', 1, @submissionDate, NULL, NULL, NULL, 0, 0 
END'

EXEC sp_executesql @sqlCommand

IF NOT EXISTS(SELECT 1 FROM [employer_levy].[TopUpPercentage] WHERE datefrom='2015-01-01 00:00:00.000' and amount=0.1 )
BEGIN
	insert into [employer_levy].[TopUpPercentage]
	(datefrom,amount)
	values
	('2015-01-01 00:00:00.000',0.1)
END

EXECUTE #CreateLevy '222/AA00002', 500
EXECUTE #CreateLevy '123/SFAT029', 1000000
EXECUTE #CreateLevy '101/CUR00016', 0

DROP PROCEDURE #CreateLevy