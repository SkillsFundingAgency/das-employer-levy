using System;

namespace SFA.DAS.EmployerLevy.Application.Queries.GetEnglishFractionUpdateRequired
{
    public class GetEnglishFractionUpdateRequiredResponse
    {
        public bool UpdateRequired { get; set; }
        public DateTime DateCalculated { get; set; }
    }
}
