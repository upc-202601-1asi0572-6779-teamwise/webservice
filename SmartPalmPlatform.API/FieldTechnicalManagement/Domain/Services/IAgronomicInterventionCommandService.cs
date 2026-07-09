using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Commands;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Services;

public interface IAgronomicInterventionCommandService
{
    Task<AgronomicIntervention> Handle(RegisterAgronomicInterventionCommand command);
}
