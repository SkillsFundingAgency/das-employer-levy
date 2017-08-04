using MediatR;

namespace SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration
{
    public class GetHMRCLevyDeclarationQuery : IAsyncRequest<GetHMRCLevyDeclarationResponse>
    {
        public string EmpRef { get; set; }
    }
}
