CREATE PROCEDURE [dbo].[DeletePayeScheme]
	@EmpRef varchar(100)
AS
	DELETE FROM PayeScheme where EmpRef = @EmpRef
