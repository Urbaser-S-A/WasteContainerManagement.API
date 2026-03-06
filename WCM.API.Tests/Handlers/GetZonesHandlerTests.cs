using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class GetZonesHandlerTests
{
    private readonly Mock<IZoneRepository> _repositoryMock = new();
    private readonly GetZonesHandler _handler;

    public GetZonesHandlerTests()
    {
        _handler = new GetZonesHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_return_mapped_dtos_when_repository_succeeds()
    {
        List<Zone> entities =
        [
            new Zone { Id = Guid.NewGuid(), Name = "Zone A", District = "Centro", City = "Madrid" },
            new Zone { Id = Guid.NewGuid(), Name = "Zone B", District = "Norte", City = "Madrid" }
        ];

        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<Zone>>(entities));

        Result<IReadOnlyList<ZoneDto>> result = await _handler.Handle(new GetZonesQuery(null, null, null), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Equal("Zone A", result.Value[0].Name);
    }

    [Fact]
    public async Task Handle_should_return_failure_when_repository_fails()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<IReadOnlyList<Zone>>(DomainErrors.Database.Error));

        Result<IReadOnlyList<ZoneDto>> result = await _handler.Handle(new GetZonesQuery(null, null, null), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Database.Error", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_pass_filters_to_repository()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync("Centro", "Madrid", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<Zone>>([]));

        await _handler.Handle(new GetZonesQuery("Centro", "Madrid", true), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync("Centro", "Madrid", true, It.IsAny<CancellationToken>()), Times.Once);
    }
}
