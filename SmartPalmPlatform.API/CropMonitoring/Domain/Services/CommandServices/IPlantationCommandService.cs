using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;

public interface IPlantationCommandService
{
    Task<Plantation> Handle(CreatePlantationCommand command);
    Task Handle(AssignSectorCommand command);
    Task Handle(RemoveSectorCommand command);
    Task<Plantation> Handle(UpdatePlantationDetailsCommand command);
}
