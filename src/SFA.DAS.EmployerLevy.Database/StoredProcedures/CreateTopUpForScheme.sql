CREATE PROCEDURE [CreateTopUpForScheme]
@empRef NVARCHAR(50)
AS

--Add the topup from the declaration
INSERT INTO LevyDeclarationTopup
	select mainUpdate.* from
	(

	select 
		DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		x.TopUp as Amount
	FROM 
		[GetLevyDeclarationAndTopUp] x
	where
		x.LevyDueYTD is not null AND x.LastSubmission = 1  AND x.EmpRef = @empRef
	union all
	select
		DATEFROMPARTS(DatePart(yyyy,GETDATE()),DatePart(MM,GETDATE()),DATEPART(dd,GETDATE())) as DateAdded,
		x.SubmissionId,
		x.SubmissionDate,
		((x.EndOfYearAdjustmentAmount * isnull(x.EnglishFraction,0))* isnull(x.TopUpPercentage,0) * -1) as Amount
	FROM 
		[GetLevyDeclarationAndTopUp] x
	where
		x.LevyDueYTD is not null and x.EndOfYearAdjustment = 1   AND x.EmpRef = @empRef
	) mainUpdate
	inner join (
		select submissionId from LevyDeclaration
	EXCEPT
		select SubmissionId from LevyDeclarationTopup
	) dervx on dervx.submissionId = mainUpdate.SubmissionId