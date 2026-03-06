using Moq;
using WCM.API.ApiService.Application.Containers.CreateContainer;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class CreateContainerHandlerTests
{
    private readonly Mock<IContainerRepository> _repositoryMock = new();
    private readonly CreateContainerHandler _handler;

    public CreateContainerHandlerTests()
    {
        _handler = new CreateContainerHandler(_repositoryMock.Object);
    }

    private static CreateContainerCommand ValidCommand() => new(
        "CNT-001", Guid.NewGuid(), Guid.NewGuid(),
        40.4168, -3.7038, "Calle Mayor 1",
        1100, ContainerStatus.Active, DateTime.UtcNow);

    [Fact]
    public async Task Handle_should_create_and_return_dto_when_valid()
    {
        _repositoryMock
            .Setup(r => r.ExistsByCodeAsync("CNT-001", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(false));

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Container>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        Result<ContainerDto> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("CNT-001", result.Value.Code);
        Assert.Equal(1100, result.Value.CapacityLiters);
    }

    [Fact]
    public async Task Handle_should_return_duplicate_error_when_code_exists()
    {
        _repositoryMock
            .Setup(r => r.ExistsByCodeAsync("CNT-001", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(true));

        Result<ContainerDto> result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Container.DuplicateCode", result.Error.Code);
    }
}
