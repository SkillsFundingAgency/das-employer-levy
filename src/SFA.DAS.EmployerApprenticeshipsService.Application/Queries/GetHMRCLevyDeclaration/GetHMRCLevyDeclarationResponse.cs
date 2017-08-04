using SFA.DAS.EmployerLevy.Domain.Models.HmrcLevy;

namespace SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationResponse
    {
        public LevyDeclarations LevyDeclarations { get; set; }
        public string Empref { get; set; }
    }
}