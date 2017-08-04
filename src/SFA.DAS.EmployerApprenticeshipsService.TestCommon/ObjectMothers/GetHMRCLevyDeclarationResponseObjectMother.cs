using SFA.DAS.EmployerLevy.Application.Queries.GetHMRCLevyDeclaration;

namespace SFA.DAS.EmployerLevy.TestCommon.ObjectMothers
{
    public static class GetHMRCLevyDeclarationResponseObjectMother
    {
        public static GetHMRCLevyDeclarationResponse Create(string empref="123avc")
        {

            var declarationResponse = new GetHMRCLevyDeclarationResponse
            {
                Empref = empref,
                LevyDeclarations = DeclarationsObjectMother.Create(empref)
            };


            return declarationResponse;
        }
    }
}