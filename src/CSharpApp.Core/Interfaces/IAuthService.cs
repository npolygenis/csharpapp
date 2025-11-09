using CSharpApp.Core.Dtos;

namespace CSharpApp.Core.Interfaces;

public interface IAuthService
{
    Task<string> Login(UserCredentials userCredentials);
}