using SmartPalmPlatform.API.IAM.Application.Internal.OutboundServices;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IAM.Application.Internal.CommandServices;

/**
 * <summary>
 *     The user command service
 * </summary>
 * <remarks>
 *     This class is used to handle user commands
 * </remarks>
 */
public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    /**
     * <summary>
     *     Handle sign in command
     * </summary>
     * <param name="command">The sign in command</param>
     * <returns>The authenticated user and the JWT token</returns>
     */
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

    /**
     * <summary>
     *     Handle sign-up command
     * </summary>
     * <param name="command">The sign-up command</param>
     * <returns>A confirmation message on successful creation.</returns>
     */
    public async Task Handle(SignUpCommand command)
    {
        try
        {
            Console.WriteLine($"[INFO] IAM [UserCommandService] Sign-up attempt for username: {command.Username}");

            if (string.IsNullOrWhiteSpace(command.Username) || string.IsNullOrWhiteSpace(command.Password))
                throw new ArgumentException("Username and password cannot be empty");

            Console.WriteLine("[INFO] IAM [UserCommandService] Checking if username exists...");
            if (userRepository.ExistsByUsername(command.Username))
            {
                Console.WriteLine($"[WARN] IAM [UserCommandService] Sign-up failed: username {command.Username} already taken");
                throw new Exception($"Username {command.Username} is already taken");
            }

            Console.WriteLine("[INFO] IAM [UserCommandService] Hashing password...");
            var hashedPassword = hashingService.HashPassword(command.Password);

            Console.WriteLine("[INFO] IAM [UserCommandService] Creating user object...");
            var validRole = Enum.TryParse<UserRole>(command.Role, true, out var role);
            if (!validRole) throw new ArgumentException("Invalid role from list: Agronomist, PalmGrower");

            if (role == UserRole.Administrator)
                throw new ArgumentException("Administrator accounts cannot be created via sign-up.");
            var user = new User(command.Username, hashedPassword, command.Email, command.FullName, role);

            Console.WriteLine("[INFO] IAM [UserCommandService] Adding user to repository...");
            await userRepository.AddAsync(user);

            Console.WriteLine("[INFO] IAM [UserCommandService] Completing transaction...");
            await unitOfWork.CompleteAsync();

            Console.WriteLine($"[INFO] IAM [UserCommandService] Sign-up successful for username: {command.Username}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] IAM [UserCommandService] Sign-up Exception: {e.GetType().Name} - {e.Message}");
            throw new Exception($"An error occurred while creating user: {e.Message}");
        }
    }
}
