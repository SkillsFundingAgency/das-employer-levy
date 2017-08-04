﻿CREATE PROCEDURE [employer_levy].[GetLevyDeclaration_ByEmpRefPayrollMonthPayrollYear]
	@empRef varchar(50),
	@payrollYear varchar(10),
	@payrollMonth int
AS
	select TOP 1
		*
	FROM 
		[employer_levy].[LevyDeclaration]
	where
		EmpRef = @empRef
	AND
		PayrollMonth = @payrollMonth
	AND
		PayrollYear = @payrollYear
	AND 
		EndOfYearAdjustment = 0
	ORDER BY 
		SubmissionDate DESC
		
	

