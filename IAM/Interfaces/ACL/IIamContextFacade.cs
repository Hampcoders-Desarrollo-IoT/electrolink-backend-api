namespace Hampcoders.Electrolink.API.IAM.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<string> FetchUserIdByEmail(string email);
    Task<string> FetchEmailByUserId(string userId);
    Task<bool> UserExistsAsync(string userId);
    Task<string?> GetUserAccessRoleAsync(string userId);
}