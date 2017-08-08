CREATE PROCEDURE [GetLevyDeclarationSubmissionIdsByEmpRef]
	@empRef NVARCHAR(50)
AS
	select 
		x.SubmissionId
	FROM 
		[GetLevyDeclarationAndTopUp] x
	where
	x.EmpRef = @empRef
	order by SubmissionDate asc

