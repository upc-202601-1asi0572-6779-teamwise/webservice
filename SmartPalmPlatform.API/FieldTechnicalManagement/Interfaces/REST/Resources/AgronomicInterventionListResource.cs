namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

public record AgronomicInterventionListResource(
    List<AgronomicInterventionResource> interventions,
    int totalCount
);
