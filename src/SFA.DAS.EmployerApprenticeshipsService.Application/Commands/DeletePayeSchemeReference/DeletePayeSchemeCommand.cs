using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediatR;

namespace SFA.DAS.EmployerLevy.Application.Commands.DeletePayeSchemeReference
{
    public class DeletePayeSchemeCommand : IAsyncRequest
    {
        public string EmpRef { get; set; }
    }
}
