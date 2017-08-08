CREATE PROCEDURE [GetLastLevyDeclarations_ByEmpRef]
	@empRef varchar(20)
AS
	select top 1
		*
	FROM 
		[LevyDeclaration]
	where
		EmpRef = @empRef
	order by SubmissionDate desc
