CREATE PROCEDURE [dbo].[UpsertPayeScheme]
	@empRef varchar(100)
AS
	MERGE [PayeScheme] AS [Target]
	USING (SELECT @empRef AS EmpRef) AS [Source] 
	ON [Target].EmpRef = [Source].EmpRef
	WHEN NOT MATCHED THEN  INSERT (EmpRef) VALUES (@empRef);