using Moq;
using WCM.API.ApiService.Application.Incidents.UpdateIncident;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class UpdateIncidentHandlerTests
{
    private readonly Mock<IIncidentRepository> _repositoryMock = new();
    private readonly UpdateIncidentHandler _handler;

    public UpdateIncidentHandlerTests()
    {
        _handler = new UpdateIncidentHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_update_when_valid()
    {
        Guid id = Guid.NewGuid();
        Incident entity = new() { Id = id, Status = IncidentStatus.Open, Type = IncidentType.Overflow };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(entity));

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        UpdateIncidentCommand command = new(id, IncidentType.Damage, "Updated", IncidentStatus.InProgress, IncidentPriority.High, null);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(IncidentType.Damage, entity.Type);
        Assert.Equal(IncidentStatus.InProgress, entity.Status);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_entity_missing()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(null));

        UpdateIncidentCommand command = new(id, IncidentType.Damage, null, IncidentStatus.Open, IncidentPriority.Low, null);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Incident.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_reject_reopening_resolved_incident()
    {
        Guid id = Guid.NewGuid();
        Incident entity = new() { Id = id, Status = IncidentStatus.Resolved, Type = IncidentType.Overflow };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(entity));

        UpdateIncidentCommand command = new(id, IncidentType.Overflow, null, IncidentStatus.Open, IncidentPriority.Medium, null);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Incident.AlreadyResolved", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_reject_reopening_closed_incident()
    {
        Guid id = Guid.NewGuid();
        Incident entity = new() { Id = id, Status = IncidentStatus.Closed, Type = IncidentType.Damage };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(entity));

        UpdateIncidentCommand command = new(id, IncidentType.Damage, null, IncidentStatus.InProgress, IncidentPriority.High, null);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Incident.AlreadyResolved", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_auto_set_resolved_at_when_resolving()
    {
        Guid id = Guid.NewGuid();
        Incident entity = new() { Id = id, Status = IncidentStatus.Open, Type = IncidentType.Overflow };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(entity));

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        UpdateIncidentCommand command = new(id, IncidentType.Overflow, null, IncidentStatus.Resolved, IncidentPriority.Medium, null);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(entity.ResolvedAt);
    }

    [Fact]
    public async Task Handle_should_keep_explicit_resolved_at_when_provided()
    {
        Guid id = Guid.NewGuid();
        Incident entity = new() { Id = id, Status = IncidentStatus.Open, Type = IncidentType.Overflow };
        DateTime explicitResolvedAt = new(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(entity));

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        UpdateIncidentCommand command = new(id, IncidentType.Overflow, null, IncidentStatus.Resolved, IncidentPriority.Medium, explicitResolvedAt);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(explicitResolvedAt, entity.ResolvedAt);
    }
}
