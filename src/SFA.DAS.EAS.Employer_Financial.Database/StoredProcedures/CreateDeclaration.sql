CREATE PROCEDURE [employer_levy].[CreateDeclaration]
	@LevyDueYtd DECIMAL (18,4), 
	@EmpRef NVARCHAR(20), 
	@SubmissionDate DATETIME, 
	@SubmissionId BIGINT, 
	@HmrcSubmissionId BIGINT,
	@LevyAllowanceForYear DECIMAL(18, 4),
	@PayrollYear NVARCHAR(10),
	@PayrollMonth TINYINT,
	@CreatedDate DATETIME,
	@DateCeased DATETIME = NULL,
	@InactiveFrom DATETIME = NULL,
	@InactiveTo DATETIME = NULL,
	@EndOfYearAdjustment BIT,
	@EndOfYearAdjustmentAmount DECIMAL(18,4)
AS
	

INSERT INTO [employer_levy].[LevyDeclaration] 
	(
		LevyDueYtd, 
		empRef, 
		SubmissionDate, 
		SubmissionId, 
		LevyAllowanceForYear,
		PayrollYear,
		PayrollMonth,
		CreatedDate,
		EndOfYearAdjustment,
		EndOfYearAdjustmentAmount,
		DateCeased,
		InactiveFrom,
		InactiveTo,
		HmrcSubmissionId
	) 
VALUES 
	(
		@LevyDueYtd, 
		@EmpRef, 
		@SubmissionDate, 
		@SubmissionId, 
		@LevyAllowanceForYear,
		@PayrollYear,
		@PayrollMonth,
		@CreatedDate,
		@EndOfYearAdjustment,
		@EndOfYearAdjustmentAmount,
		@DateCeased,
		@InactiveFrom,
		@InactiveTo,
		@HmrcSubmissionId
	);