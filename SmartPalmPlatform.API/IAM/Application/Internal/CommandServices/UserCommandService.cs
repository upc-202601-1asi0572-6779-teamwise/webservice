using SmartPalmPlatform.API.IAM.Application.Internal.OutboundServices;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IAM.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUserDomainService userDomainService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    public async Task<(User user, string token)> Handle(SignInCommand command)
    {
        Console.WriteLine($"[INFO] IAM [UserCommandService] Sign-in attempt for username: {command.Username}");

        var user = await userRepository.FindByUsernameAsync(command.Username);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
        {
            Console.WriteLine($"[WARN] IAM [UserCommandService] Sign-in failed for username: {command.Username} - invalid credentials");
            throw new Exception("Invalid username or password");
        }

        var token = tokenService.GenerateToken(user);

        return (user, token);
    }

    public async Task<User> Handle(CreateUserCommand command)
    {
        Console.WriteLine($"[INFO] IAM [UserCommandService] Admin create user: {command.Username}");

        userDomainService.ValidateUsername(command.Username);
        userDomainService.ValidatePassword(command.Password);
        userDomainService.ValidateEmail(command.Email);
        var role = userDomainService.ParseUserRole(command.Role);

        if (userRepository.ExistsByUsername(command.Username))
            throw new ArgumentException($"Username '{command.Username}' is already taken.");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Username, hashedPassword, command.Email, command.FullName, role);

        await userRepository.AddAsync(user);
        await unitOfWork.CompleteAsync();

        Console.WriteLine($"[INFO] IAM [UserCommandService] User created: {command.Username} (role={role})");
        return user;
    }
}
