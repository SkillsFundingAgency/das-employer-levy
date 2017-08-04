CREATE PROCEDURE [employer_levy].[GetLastLevyDeclarations_ByEmpRef]
	@empRef varchar(20)
AS
	select top 1
		*
	FROM 
		[employer_levy].[LevyDeclaration]
	where
		EmpRef = @empRef
	order by SubmissionDate desc
