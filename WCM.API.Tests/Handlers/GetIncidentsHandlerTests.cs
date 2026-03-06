using Moq;
using WCM.API.ApiService.Application.Incidents.GetIncidents;
using WCM.API.ApiService.Application.Interfaces;
using WCM.API.ApiService.Domain.Entities;
using WCM.API.ApiService.Domain.Enums;
using WCM.API.ApiService.Domain.Shared;

namespace WCM.API.Tests.Handlers;

public class GetIncidentsHandlerTests
{
    private readonly Mock<IIncidentRepository> _repositoryMock = new();
    private readonly GetIncidentsHandler _handler;

    public GetIncidentsHandlerTests()
    {
        _handler = new GetIncidentsHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_should_return_paginated_response()
    {
        List<Incident> entities =
        [
            new Incident { Id = Guid.NewGuid(), ContainerId = Guid.NewGuid(), Type = IncidentType.Overflow, Status = IncidentStatus.Open, Container = new Container { Code = "CNT-001" } },
            new Incident { Id = Guid.NewGuid(), ContainerId = Guid.NewGuid(), Type = IncidentType.Damage, Status = IncidentStatus.InProgress, Container = new Container { Code = "CNT-002" } }
        ];

        _repositoryMock
            .Setup(r => r.GetTotalCountAsync(null, null, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(2));

        _repositoryMock
            .Setup(r => r.GetAllAsync(null, null, null, null, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<IReadOnlyList<Incident>>(entities));

        GetIncidentsQuery query = new(null, null, null, null, null, null, 1, 20);
        Result<IncidentListResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Items.Count);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal("CNT-001", result.Value.Items[0].ContainerCode);
    }

    [Fact]
    public async Task Handle_should_return_failure_when_count_fails()
    {
        _repositoryMock
            .Setup(r => r.GetTotalCountAsync(
                It.IsAny<Guid?>(), It.IsAny<IncidentType?>(), It.IsAny<IncidentStatus?>(),
                It.IsAny<IncidentPriority?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<int>(DomainErrors.Database.Error));

        GetIncidentsQuery query = new(null, null, null, null, null, null, 1, 20);
        Result<IncidentListResponse> result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
    }
}
