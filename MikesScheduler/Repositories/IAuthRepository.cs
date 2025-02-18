namespace MikesScheduler.Repositories;

public interface IAuthRepository
{
    public Task<Auth> Login(string username, string password);
}
