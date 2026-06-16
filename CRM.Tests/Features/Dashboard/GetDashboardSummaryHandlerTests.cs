using CRM.Application.Features.Dashboard;
using CRM.Application.Features.Dashboard.GetDashboardSummary;
using CRM.Application.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CRM.Tests.Features.Dashboard;

[TestFixture]
public class GetDashboardSummaryHandlerTests
{
    private Mock<IDashboardRepository> _repo = null!;
    private GetDashboardSummaryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<IDashboardRepository>();
        _handler = new GetDashboardSummaryHandler(_repo.Object);
    }

    [Test]
    public async Task Handle_ReturnsDtoFromRepository()
    {
        var expected = new DashboardSummaryDto(5, 3, new TicketsByStatusDto(2, 1, 0, 4, 6));
        _repo.Setup(r => r.GetSummaryAsync(default)).ReturnsAsync(expected);

        var result = await _handler.Handle(new GetDashboardSummaryQuery(), default);

        result.Should().Be(expected);
        result.CurrentMonthLeads.Should().Be(5);
        result.ConvertedCustomersThisMonth.Should().Be(3);
        result.TicketsByStatus.New.Should().Be(2);
    }

    [Test]
    public async Task Handle_CallsRepositoryExactlyOnce()
    {
        _repo.Setup(r => r.GetSummaryAsync(default))
             .ReturnsAsync(new DashboardSummaryDto(0, 0, new TicketsByStatusDto(0, 0, 0, 0, 0)));

        await _handler.Handle(new GetDashboardSummaryQuery(), default);

        _repo.Verify(r => r.GetSummaryAsync(default), Times.Once);
    }
}
