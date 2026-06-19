namespace Hampcoders.Electrolink.API.IAM.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<string> CreateUser(string email, string password, string passwordConfirmation, string role = "User");
    Task<string> FetchUserIdByEmail(string email);
    Task<string> FetchEmailByUserId(string userId);
    Task<bool> UserExistsAsync(string userId);
}