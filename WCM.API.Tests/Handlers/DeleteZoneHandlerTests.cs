using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.Zones.DeleteZone;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class DeleteZoneHandlerTests
{
    private readonly Mock<IZoneRepository> _repositoryMock = new();
    private readonly DeleteZoneHandler _handler;

    public DeleteZoneHandlerTests()
    {
        _handler = new DeleteZoneHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_delete_when_no_active_containers()
    {
        Guid id = Guid.NewGuid();
        Zone entity = new() { Id = id, Name = "Zone A" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Zone?>(entity));

        _repositoryMock
            .Setup(r => r.HasActiveContainersAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        Result result = await _handler.Handle(new DeleteZoneCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_entity_missing()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Zone?>(null));

        Result result = await _handler.Handle(new DeleteZoneCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Zone.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_return_error_when_has_active_containers()
    {
        Guid id = Guid.NewGuid();
        Zone entity = new() { Id = id, Name = "Zone A" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Zone?>(entity));

        _repositoryMock
            .Setup(r => r.HasActiveContainersAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        Result result = await _handler.Handle(new DeleteZoneCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Zone.HasActiveContainers", result.Error.Code);
    }
}
