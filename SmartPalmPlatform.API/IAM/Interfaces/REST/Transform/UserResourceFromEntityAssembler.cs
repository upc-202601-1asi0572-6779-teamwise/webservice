using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        return new UserResource(user.Id, user.Username);
    }
}