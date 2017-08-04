namespace SFA.DAS.EmployerLevy.Domain.Interfaces
{
    public interface IHashingService
    {
        string HashValue(long id);
        long DecodeValue(string id);
    }
}
