using MediatR;
using SFA.DAS.EmployerLevy.Application.Queries.GetEnglishFractionUpdateRequired;

namespace SFA.DAS.EmployerLevy.Application.Commands.UpdateEnglishFractions
{
    public class UpdateEnglishFractionsCommand : IAsyncRequest
    {
        public string EmployerReference { get; set; }
        public GetEnglishFractionUpdateRequiredResponse EnglishFractionUpdateResponse { get; set; }
    }
}
