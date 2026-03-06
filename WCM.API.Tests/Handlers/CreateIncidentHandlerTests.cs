using Moq;
using WCM.API.ApiService.Application.Incidents.CreateIncident;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class CreateIncidentHandlerTests
{
    private readonly Mock<IIncidentRepository> _incidentRepositoryMock = new();
    private readonly Mock<IContainerRepository> _containerRepositoryMock = new();
    private readonly CreateIncidentHandler _handler;

    public CreateIncidentHandlerTests()
    {
        _handler = new CreateIncidentHandler(_incidentRepositoryMock.Object, _containerRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_create_incident_when_container_is_active()
    {
        Guid containerId = Guid.NewGuid();
        Container container = new() { Id = containerId, Code = "CNT-001", Status = ContainerStatus.Active };

        _containerRepositoryMock
            .Setup(r => r.GetByIdAsync(containerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Container?>(container));

        _incidentRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        CreateIncidentCommand command = new(containerId, IncidentType.Overflow, "Overflowing", IncidentPriority.High);
        Result<IncidentDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(IncidentType.Overflow, result.Value.Type);
        Assert.Equal(IncidentStatus.Open, result.Value.Status);
        Assert.Equal(IncidentPriority.High, result.Value.Priority);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_container_missing()
    {
        Guid containerId = Guid.NewGuid();

        _containerRepositoryMock
            .Setup(r => r.GetByIdAsync(containerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Container?>(null));

        CreateIncidentCommand command = new(containerId, IncidentType.Damage, null, IncidentPriority.Medium);
        Result<IncidentDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Container.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_return_error_when_container_is_not_active()
    {
        Guid containerId = Guid.NewGuid();
        Container container = new() { Id = containerId, Code = "CNT-001", Status = ContainerStatus.Inactive };

        _containerRepositoryMock
            .Setup(r => r.GetByIdAsync(containerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Container?>(container));

        CreateIncidentCommand command = new(containerId, IncidentType.Damage, null, IncidentPriority.Medium);
        Result<IncidentDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Incident.ContainerNotActive", result.Error.Code);
    }
}
