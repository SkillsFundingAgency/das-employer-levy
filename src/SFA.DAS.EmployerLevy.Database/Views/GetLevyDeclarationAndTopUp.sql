﻿CREATE VIEW [GetLevyDeclarationAndTopUp]
AS

SELECT *,
	   (decPlusLevy.LevyDeclaredInMonth * decPlusLevy.EnglishFraction) + decPlusLevy.TopUp as TotalAmount
FROM
	(SELECT *,
		   (dec.LevyDeclaredInMonth * dec.EnglishFraction) * dec.TopUpPercentage as TopUp
	FROM
		(SELECT 
			ld.*,
			ld.LevyDueYTD - ISNULL(y.LevyDueYTD, 0) AS LevyDeclaredInMonth
		FROM [GetLevyDeclaration] ld
		outer apply
		(
			SELECT TOP 1 LevyDueYTD
			FROM [GetLevyDeclaration] y
			WHERE y.EmpRef = ld.empRef AND y.PayrollYear = ld.PayrollYear AND y.LastSubmission = 1 AND y.PayrollMonth < ld.PayrollMonth AND y.LevyDueYTD IS NOT NULL
			ORDER BY y.PayrollMonth DESC
		) y
	) dec
) decPlusLevy

GO