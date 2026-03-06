using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class GetWasteTypeByIdHandlerTests
{
    private readonly Mock<IWasteTypeRepository> _repositoryMock = new();
    private readonly GetWasteTypeByIdHandler _handler;

    public GetWasteTypeByIdHandlerTests()
    {
        _handler = new GetWasteTypeByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_return_dto_when_found()
    {
        Guid id = Guid.NewGuid();
        WasteType entity = new() { Id = id, Name = "Organic", IsActive = true };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(entity));

        Result<WasteTypeDto> result = await _handler.Handle(new GetWasteTypeByIdQuery(id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("Organic", result.Value.Name);
    }

    [Fact]
    public async Task Handle_should_return_not_found_when_null()
    {
        Guid id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WasteType?>(null));

        Result<WasteTypeDto> result = await _handler.Handle(new GetWasteTypeByIdQuery(id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("WasteType.NotFound", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_return_failure_when_repository_fails()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<WasteType?>(DomainErrors.Database.Error));

        Result<WasteTypeDto> result = await _handler.Handle(new GetWasteTypeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Database.Error", result.Error.Code);
    }
}
