using Moq;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Application.Zones.CreateZone;
using WCM.API.ApiService.Application.Zones.GetZones;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class CreateZoneHandlerTests
{
    private readonly Mock<IZoneRepository> _repositoryMock = new();
    private readonly CreateZoneHandler _handler;

    public CreateZoneHandlerTests()
    {
        _handler = new CreateZoneHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_create_and_return_dto_when_valid()
    {
        _repositoryMock
            .Setup(r => r.ExistsByNameAsync("Zone A", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Zone>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        CreateZoneCommand command = new("Zone A", "Centro", "Madrid", true);
        Result<ZoneDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Zone A", result.Value.Name);
        Assert.Equal("Centro", result.Value.District);
    }

    [Fact]
    public async Task Handle_should_return_duplicate_error_when_name_exists()
    {
        _repositoryMock
            .Setup(r => r.ExistsByNameAsync("Zone A", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        CreateZoneCommand command = new("Zone A", null, null, true);
        Result<ZoneDto> result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Zone.DuplicateName", result.Error.Code);
    }
}
