using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class UpdateWasteTypeHandlerTests
{
    private readonly Mock<IWasteTypeRepository> _repositoryMock = new();
    private readonly UpdateWasteTypeHandler _handler;

    public UpdateWasteTypeHandlerTests()
    {
        _handler = new UpdateWasteTypeHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_update_when_valid()
    {
        Guid id = Guid.NewGuid();
        WasteType entity = new() { Id = id, Name = "Old Name" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(entity));

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync("New Name", id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<WasteType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        UpdateWasteTypeCommand command = new(id, "New Name", "Desc", "#FF0000", true);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New Name", entity.Name);
        Assert.Equal("#FF0000", entity.ColorCode);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_entity_missing()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(null));

        UpdateWasteTypeCommand command = new(id, "Name", null, null, true);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("WasteType.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_return_duplicate_error_when_name_taken()
    {
        Guid id = Guid.NewGuid();
        WasteType entity = new() { Id = id, Name = "Old Name" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(entity));

        _repositoryMock
            .Setup(r => r.ExistsByNameAsync("Taken Name", id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        UpdateWasteTypeCommand command = new(id, "Taken Name", null, null, true);
        Result result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("WasteType.DuplicateName", result.Error.Code);
    }
}
