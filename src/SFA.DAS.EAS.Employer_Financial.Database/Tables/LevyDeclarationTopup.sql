CREATE TABLE [employer_levy].LevyDeclarationTopup
(
	[Id] BIGINT identity (1,1) not null,
	[DateAdded] DATETIME not null,
	[SubmissionId] BIGINT not null,
	[SubmissionDate] DATETIME not null,
	[Amount] DECIMAL (18,4) not null default 0
)
GO

CREATE INDEX [IX_LevyDeclarationtopup_submissionid] ON [employer_levy].[LevyDeclarationTopup] (submissionId)