using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.CreateWasteType;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class CreateWasteTypeHandlerTests
{
    private readonly Mock<IWasteTypeRepository> _repositoryMock = new();
    private readonly CreateWasteTypeHandler _handler;

    public CreateWasteTypeHandlerTests()
    {
        _handler = new CreateWasteTypeHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_create_and_return_dto_when_valid()
    {
        _repositoryMock
            .Setup(r => r.ExistsByNameAsync("Organic", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<WasteType>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        CreateWasteTypeCommand command = new("Organic", "Organic waste", "#00FF00", true);
        Result<WasteTypeDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Organic", result.Value.Name);
        Assert.Equal("#00FF00", result.Value.ColorCode);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<WasteType>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_should_return_duplicate_error_when_name_exists()
    {
        _repositoryMock
            .Setup(r => r.ExistsByNameAsync("Organic", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        CreateWasteTypeCommand command = new("Organic", null, null, true);
        Result<WasteTypeDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("WasteType.DuplicateName", result.Error.Code);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<WasteType>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_should_return_failure_when_exists_check_fails()
    {
        _repositoryMock
            .Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool>(DomainErrors.Database.Error));

        CreateWasteTypeCommand command = new("Organic", null, null, true);
        Result<WasteTypeDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Database.Error", result.Error.Code);
    }
}
