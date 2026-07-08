using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class RoleFromStringAssembler
{
    public static Role FromStringToRole(string role)
    {
        return role switch
        {
            "Agronomist" => Role.Agronomist,
            "Farmer" => Role.Farmer,
            "Administrator" => Role.Administrator,
            _ => throw new ArgumentException($"Unknown role: '{role}'."),
        };
    }
}
