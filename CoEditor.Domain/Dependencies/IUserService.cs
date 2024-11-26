namespace CoEditor.Domain.Dependencies;

public interface IUserService
{
    Task<string> GetUserNameAsync();
}
