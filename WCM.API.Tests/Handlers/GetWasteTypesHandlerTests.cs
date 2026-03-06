using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class GetWasteTypesHandlerTests
{
    private readonly Mock<IWasteTypeRepository> _repositoryMock = new();
    private readonly GetWasteTypesHandler _handler;

    public GetWasteTypesHandlerTests()
    {
        _handler = new GetWasteTypesHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_return_mapped_dtos_when_repository_succeeds()
    {
        List<WasteType> entities =
        [
            new WasteType { Id = Guid.NewGuid(), Name = "Organic", IsActive = true },
            new WasteType { Id = Guid.NewGuid(), Name = "Plastic", IsActive = true }
        ];

        _repositoryMock
            .Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<WasteType>>(entities));

        Result<IReadOnlyList<WasteTypeDto>> result = await _handler.Handle(new GetWasteTypesQuery(null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Organic", result.Value[0].Name);
        Assert.Equal("Plastic", result.Value[1].Name);
    }

    [Fact]
    public async Task Handle_should_return_failure_when_repository_fails()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<IReadOnlyList<WasteType>>(DomainErrors.Database.Error));

        Result<IReadOnlyList<WasteTypeDto>> result = await _handler.Handle(new GetWasteTypesQuery(null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Database.Error", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_pass_isActive_filter_to_repository()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<WasteType>>([]));

        await _handler.Handle(new GetWasteTypesQuery(true), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync(true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
