using Moq;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class GetContainersHandlerTests
{
    private readonly Mock<IContainerRepository> _repositoryMock = new();
    private readonly GetContainersHandler _handler;

    public GetContainersHandlerTests()
    {
        _handler = new GetContainersHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_return_paginated_response()
    {
        List<Container> entities =
        [
            new Container { Id = Guid.NewGuid(), Code = "CNT-001", WasteType = new WasteType { Name = "Organic" }, Zone = new Zone { Name = "Zone A" } },
            new Container { Id = Guid.NewGuid(), Code = "CNT-002", WasteType = new WasteType { Name = "Plastic" }, Zone = new Zone { Name = "Zone B" } }
        ];

        _repositoryMock
            .Setup(r => r.GetTotalCountAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(2));

        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<Container>>(entities));

        GetContainersQuery query = new(null, null, null, 1, 20);
        Result<ContainerListResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(20, result.Value.PageSize);
        Assert.Equal("Organic", result.Value.Items[0].WasteTypeName);
    }

    [Fact]
    public async Task Handle_should_return_failure_when_count_fails()
    {
        _repositoryMock
            .Setup(r => r.GetTotalCountAsync(It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<ContainerStatus?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>(DomainErrors.Database.Error));

        GetContainersQuery query = new(null, null, null, 1, 20);
        Result<ContainerListResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Database.Error", result.Error.Code);
    }

    [Fact]
    public async Task Handle_should_pass_filters_to_repository()
    {
        Guid zoneId = Guid.NewGuid();
        Guid wasteTypeId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetTotalCountAsync(zoneId, wasteTypeId, ContainerStatus.Active, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(0));

        _repositoryMock
            .Setup(r => r.GetAllAsync(zoneId, wasteTypeId, ContainerStatus.Active, 2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<Container>>([]));

        GetContainersQuery query = new(zoneId, wasteTypeId, ContainerStatus.Active, 2, 10);
        await _handler.Handle(query, CancellationToken.None);

        _repositoryMock.Verify(r => r.GetTotalCountAsync(zoneId, wasteTypeId, ContainerStatus.Active, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.GetAllAsync(zoneId, wasteTypeId, ContainerStatus.Active, 2, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}
