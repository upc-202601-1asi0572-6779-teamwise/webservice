using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;

namespace SmartPalmPlatform.API.IAM.Domain.Services.CommandServices;

public interface IPaymentCommandService
{
    Task<PaymentTransaction> Handle(ProcessPaymentCommand command);
}
