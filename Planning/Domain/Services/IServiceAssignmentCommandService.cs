using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IServiceAssignmentCommandService
{
    Task Handle(ExecuteMatchingAlgorithmCommand command);
    Task Handle(AssignStaffToServiceCommand command);
}
