using Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;
using Hampcoders.Electrolink.API.IAM.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.IAM.Interfaces.REST.Transform;

public static class RequestPasswordResetCommandFromResourceAssembler
{
    public static RequestPasswordResetCommand ToCommandFromResource(RequestPasswordResetResource resource) =>
        new(resource.Email);
}
