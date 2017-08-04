﻿CREATE VIEW [employer_levy].[GetLevyDeclaration]
AS 
SELECT 
	ld.Id AS Id,	
	ld.empRef AS EmpRef,
	ld.SubmissionDate AS SubmissionDate,
	ld.SubmissionId AS SubmissionId,
	ld.LevyDueYTD AS LevyDueYTD,
	coalesce(efo.Amount, t.Amount, 1) AS EnglishFraction,
	w.amount as TopUpPercentage,
	ld.PayrollYear as PayrollYear,
	ld.PayrollMonth as PayrollMonth,
	CASE 
		x.submissionid 
	when 
		ld.submissionid then 1 
	else 0 end as LastSubmission,
	ld.CreatedDate,
	ld.EndOfYearAdjustment,
	ld.EndOfYearAdjustmentAmount,
	ld.LevyAllowanceForYear AS LevyAllowanceForYear,
	ld.DateCeased AS DateCeased,
	ld.InactiveFrom AS InactiveFrom,
	ld.InactiveTo AS InactiveTo,
	ld.HmrcSubmissionId AS HmrcSubmissionId
FROM [employer_levy].[LevyDeclaration] ld
left join
(	
	select max(submissionid) submissionid,empRef,PayrollMonth,PayrollYear from
	[employer_levy].LevyDeclaration xld
	where submissiondate in 
		(select max(submissiondate) from [employer_levy].LevyDeclaration WHERE EndOfYearAdjustment = 0 
		and submissiondate < [employer_levy].[CalculateSubmissionCutoffDate](PayrollMonth, PayrollYear)
		and PayrollYear = xld.PayrollYear
		and PayrollMonth = xld.PayrollMonth
		and empRef = xld.empRef
		group by empRef,PayrollYear,PayrollMonth)
	and submissiondate < [employer_levy].[CalculateSubmissionCutoffDate](PayrollMonth, PayrollYear)
		group by empRef,PayrollYear,PayrollMonth
)x on x.empRef = ld.empRef and x.PayrollMonth = ld.PayrollMonth and x.PayrollYear = ld.PayrollYear AND ld.EndOfYearAdjustment = 0

OUTER APPLY
(
	SELECT TOP 1 Amount
	FROM [employer_levy].[EnglishFraction] ef
	WHERE ef.EmpRef = ld.empRef 
		AND ef.[DateCalculated] < [employer_levy].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
	ORDER BY [DateCalculated] DESC
) t
outer apply
(
	SELECT top 1 Amount
	from [employer_levy].[TopUpPercentage] tp
	WHERE tp.[DateFrom] < [employer_levy].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
) w
outer apply
(
	SELECT top 1 Amount
	FROM [employer_levy].[EnglishFractionOverride] o
	WHERE o.EmpRef = ld.empref AND o.DateFrom < [employer_levy].[CalculateSubmissionCutoffDate](ld.PayrollMonth, ld.PayrollYear)
	ORDER BY DateFrom DESC
) efo