namespace SFA.DAS.EmployerLevy.Domain.Http
{
    public class RequestTimeOutException : HttpException
    {
        public RequestTimeOutException() : base(408, "Request has time out")
        {
        }
    }
}