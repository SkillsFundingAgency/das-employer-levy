using MediatR;

namespace SFA.DAS.EmployerLevy.Application.Commands.CreatePayeSchemeReference
{
    public class CreatePayeSchemeCommand : IAsyncRequest
    {
        public string EmpRef { get; set; }
    }
}