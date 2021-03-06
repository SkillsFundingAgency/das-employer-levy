﻿using System;

namespace SFA.DAS.EmployerLevy.Domain.Models.Levy
{
    public class LevyDeclarationSourceDataItem
    {
        public long Id { get; set; }
        public string EmpRef { get; set; }
        public decimal LevyDueYtd { get; set; }
        public decimal EnglishFraction { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? PayrollDate { get; set; }
        public string PayrollYear { get; set; }
        public short? PayrollMonth { get; set; }
        public int LastSubmission { get; set; }
        public decimal TopUp { get; set; }
        public DateTime EmprefAddedDate { get; set; }
        public DateTime? EmprefRemovedDate { get; set; }
    }
}