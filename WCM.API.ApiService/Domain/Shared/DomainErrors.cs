namespace WCM.API.ApiService.Domain.Shared;

/// <summary>
/// Centralized catalog of all domain errors in the application.
/// Each error contains a code, message, and HTTP status code.
/// </summary>
public static class DomainErrors
{
    public static class Database
    {
        public static Error Timeout => new(
            "Database.Timeout",
            "The operation has timed out. Please try again later.",
            StatusCodes.Status408RequestTimeout);

        public static Error Error => new(
            "Database.Error",
            "An error occurred while querying the database. Please contact the administrator.",
            StatusCodes.Status500InternalServerError);
    }

    public static class Validation
    {
        public static Error InvalidContainerStatus(string status) => new(
            "Validation.InvalidContainerStatus",
            $"The container status '{status}' is not valid.",
            StatusCodes.Status400BadRequest);

        public static Error InvalidIncidentType(string incidentType) => new(
            "Validation.InvalidIncidentType",
            $"The incident type '{incidentType}' is not valid.",
            StatusCodes.Status400BadRequest);

        public static Error InvalidIncidentStatus(string status) => new(
            "Validation.InvalidIncidentStatus",
            $"The incident status '{status}' is not valid.",
            StatusCodes.Status400BadRequest);

        public static Error InvalidPriority(string priority) => new(
            "Validation.InvalidPriority",
            $"The priority '{priority}' is not valid.",
            StatusCodes.Status400BadRequest);
    }

    public static class WasteTypes
    {
        public static Error NotFound(Guid id) => new(
            "WasteType.NotFound",
            $"The waste type with ID '{id}' was not found.",
            StatusCodes.Status404NotFound);

        public static Error DuplicateName(string name) => new(
            "WasteType.DuplicateName",
            $"A waste type with the name '{name}' already exists.",
            StatusCodes.Status409Conflict);

        public static Error HasActiveContainers(Guid id) => new(
            "WasteType.HasActiveContainers",
            $"The waste type with ID '{id}' cannot be deleted because it has active containers.",
            StatusCodes.Status422UnprocessableEntity);
    }

    public static class Zones
    {
        public static Error NotFound(Guid id) => new(
            "Zone.NotFound",
            $"The zone with ID '{id}' was not found.",
            StatusCodes.Status404NotFound);

        public static Error DuplicateName(string name) => new(
            "Zone.DuplicateName",
            $"A zone with the name '{name}' already exists.",
            StatusCodes.Status409Conflict);

        public static Error HasActiveContainers(Guid id) => new(
            "Zone.HasActiveContainers",
            $"The zone with ID '{id}' cannot be deleted because it has active containers.",
            StatusCodes.Status422UnprocessableEntity);
    }

    public static class Containers
    {
        public static Error NotFound(Guid id) => new(
            "Container.NotFound",
            $"The container with ID '{id}' was not found.",
            StatusCodes.Status404NotFound);

        public static Error DuplicateCode(string code) => new(
            "Container.DuplicateCode",
            $"A container with the code '{code}' already exists.",
            StatusCodes.Status409Conflict);

        public static Error HasOpenIncidents(Guid id) => new(
            "Container.HasOpenIncidents",
            $"The container with ID '{id}' cannot be deleted because it has open incidents.",
            StatusCodes.Status422UnprocessableEntity);

        public static Error InvalidStatusTransition(string currentStatus, string newStatus) => new(
            "Container.InvalidStatusTransition",
            $"Cannot transition container from '{currentStatus}' to '{newStatus}'.",
            StatusCodes.Status422UnprocessableEntity);
    }

    public static class Incidents
    {
        public static Error NotFound(Guid id) => new(
            "Incident.NotFound",
            $"The incident with ID '{id}' was not found.",
            StatusCodes.Status404NotFound);

        public static Error ContainerNotActive(Guid containerId) => new(
            "Incident.ContainerNotActive",
            $"Cannot create an incident for container '{containerId}' because it is not active.",
            StatusCodes.Status422UnprocessableEntity);

        public static Error AlreadyResolved(Guid id) => new(
            "Incident.AlreadyResolved",
            $"The incident with ID '{id}' has already been resolved.",
            StatusCodes.Status422UnprocessableEntity);
    }

    public static class General
    {
        public static Error Error => new(
            "General.Error",
            "An unexpected error occurred. Please contact the administrator.",
            StatusCodes.Status500InternalServerError);

        public static Error NotFound => new(
            "General.NotFound",
            "The requested resource was not found.",
            StatusCodes.Status404NotFound);
    }
}
