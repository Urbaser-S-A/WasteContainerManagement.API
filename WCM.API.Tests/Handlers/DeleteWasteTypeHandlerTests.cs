using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class DeleteWasteTypeHandlerTests
{
    private readonly Mock<IWasteTypeRepository> _repositoryMock = new();
    private readonly DeleteWasteTypeHandler _handler;

    public DeleteWasteTypeHandlerTests()
    {
        _handler = new DeleteWasteTypeHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_delete_when_no_active_containers()
    {
        Guid id = Guid.NewGuid();
        WasteType entity = new() { Id = id, Name = "Organic" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(entity));

        _repositoryMock
            .Setup(r => r.HasActiveContainersAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        Result result = await _handler.Handle(new DeleteWasteTypeCommand(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_entity_missing()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(null));

        Result result = await _handler.Handle(new DeleteWasteTypeCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("WasteType.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_return_error_when_has_active_containers()
    {
        Guid id = Guid.NewGuid();
        WasteType entity = new() { Id = id, Name = "Organic" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(entity));

        _repositoryMock
            .Setup(r => r.HasActiveContainersAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        Result result = await _handler.Handle(new DeleteWasteTypeCommand(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("WasteType.HasActiveContainers", result.Error.Code);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<WasteType>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
