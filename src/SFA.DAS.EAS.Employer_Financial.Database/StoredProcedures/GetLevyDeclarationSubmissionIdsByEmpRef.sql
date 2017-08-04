CREATE PROCEDURE [employer_levy].[GetLevyDeclarationSubmissionIdsByEmpRef]
	@empRef NVARCHAR(50)
AS
	select 
		x.SubmissionId
	FROM 
		[employer_levy].[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef = @empRef
	order by SubmissionDate asc

