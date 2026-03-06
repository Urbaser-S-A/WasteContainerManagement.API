using Moq;
using WCM.API.ApiService.Application.Containers.DeleteContainer;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class DeleteContainerHandlerTests
{
    private readonly Mock<IContainerRepository> _repositoryMock = new();
    private readonly DeleteContainerHandler _handler;

    public DeleteContainerHandlerTests()
    {
        _handler = new DeleteContainerHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_delete_when_no_open_incidents()
    {
        Guid id = Guid.NewGuid();
        Container entity = new() { Id = id, Code = "CNT-001" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Container?>(entity));

        _repositoryMock
            .Setup(r => r.HasOpenIncidentsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        Result result = await _handler.Handle(new DeleteContainerCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_entity_missing()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Container?>(null));

        Result result = await _handler.Handle(new DeleteContainerCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Container.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_return_error_when_has_open_incidents()
    {
        Guid id = Guid.NewGuid();
        Container entity = new() { Id = id, Code = "CNT-001" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Container?>(entity));

        _repositoryMock
            .Setup(r => r.HasOpenIncidentsAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        Result result = await _handler.Handle(new DeleteContainerCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Container.HasOpenIncidents", result.Error.Code);
    }
}
