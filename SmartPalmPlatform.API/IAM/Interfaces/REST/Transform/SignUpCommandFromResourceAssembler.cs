using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        return new SignUpCommand(resource.username, resource.password, resource.email, resource.fullName, resource.role);
    }
}