using Moq;
using WCM.API.ApiService.Application.Incidents.DeleteIncident;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class DeleteIncidentHandlerTests
{
    private readonly Mock<IIncidentRepository> _repositoryMock = new();
    private readonly DeleteIncidentHandler _handler;

    public DeleteIncidentHandlerTests()
    {
        _handler = new DeleteIncidentHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_delete_when_found()
    {
        Guid id = Guid.NewGuid();
        Incident entity = new() { Id = id, Type = IncidentType.Overflow, Status = IncidentStatus.Open };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(entity));

        _repositoryMock
            .Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        Result result = await _handler.Handle(new DeleteIncidentCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_entity_missing()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Incident?>(null));

        Result result = await _handler.Handle(new DeleteIncidentCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Incident.NotFound", result.Error.Code);
    }
}
