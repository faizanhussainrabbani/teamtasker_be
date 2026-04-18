using FluentAssertions;
using TeamTasker.Application.Tasks.Common;
using TeamTasker.Domain.Entities;
using Xunit;
using DomainTaskPriority = TeamTasker.Domain.Entities.TaskPriority;
using DomainTaskStatus = TeamTasker.Domain.Entities.TaskStatus;

namespace TeamTasker.Application.UnitTests.Tasks
{
    public class TaskContractMapperTests
    {
        [Theory]
        [InlineData(DomainTaskStatus.ToDo, "todo")]
        [InlineData(DomainTaskStatus.InProgress, "in-progress")]
        [InlineData(DomainTaskStatus.Done, "completed")]
        [InlineData(DomainTaskStatus.Blocked, "blocked")]
        [InlineData(DomainTaskStatus.OnHold, "on-hold")]
        [InlineData(DomainTaskStatus.Cancelled, "cancelled")]
        public void ToClientStatus_ShouldMapDomainStatus(DomainTaskStatus input, string expected)
        {
            TaskContractMapper.ToClientStatus(input).Should().Be(expected);
        }

        [Theory]
        [InlineData("todo", DomainTaskStatus.ToDo)]
        [InlineData("in-progress", DomainTaskStatus.InProgress)]
        [InlineData("completed", DomainTaskStatus.Done)]
        [InlineData("blocked", DomainTaskStatus.Blocked)]
        [InlineData("on-hold", DomainTaskStatus.OnHold)]
        [InlineData("cancelled", DomainTaskStatus.Cancelled)]
        public void TryParseStatus_ShouldParseClientStatus(string input, DomainTaskStatus expected)
        {
            var success = TaskContractMapper.TryParseStatus(input, out var parsedStatus);

            success.Should().BeTrue();
            parsedStatus.Should().Be(expected);
        }

        [Theory]
        [InlineData("low", DomainTaskPriority.Low)]
        [InlineData("medium", DomainTaskPriority.Medium)]
        [InlineData("high", DomainTaskPriority.High)]
        [InlineData("critical", DomainTaskPriority.Critical)]
        public void TryParsePriority_ShouldParseClientPriority(string input, DomainTaskPriority expected)
        {
            var success = TaskContractMapper.TryParsePriority(input, out var parsedPriority);

            success.Should().BeTrue();
            parsedPriority.Should().Be(expected);
        }

        [Theory]
        [InlineData(DomainTaskPriority.Low, "low")]
        [InlineData(DomainTaskPriority.Medium, "medium")]
        [InlineData(DomainTaskPriority.High, "high")]
        [InlineData(DomainTaskPriority.Critical, "critical")]
        public void ToClientPriority_ShouldMapDomainPriority(DomainTaskPriority input, string expected)
        {
            TaskContractMapper.ToClientPriority(input).Should().Be(expected);
        }
    }
}
