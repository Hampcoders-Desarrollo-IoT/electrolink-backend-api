using Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;

namespace Hampcoders.Electrolink.API.IAM.Domain.Services;

public interface IStaffUserCommandService
{
    Task<string> Handle(CreateStaffUserCommand command);
}
