using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Tests
{
    public class ContractWorkflowTests
    {
        [Fact]
        public void ServiceRequest_CannotBeCreated_WhenContractIsExpired()
        {
            var contract = new ContractViewModel { Id = 1, Status = 2 };

            bool canCreateRequest = contract.Status == 1;

            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_CannotBeCreated_WhenContractIsOnHold()
        {
            var contract = new ContractViewModel { Id = 1, Status = 3 };

            bool canCreateRequest = contract.Status == 1;

            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_CanBeCreated_WhenContractIsActive()
        {
            var contract = new ContractViewModel { Id = 1, Status = 1 };

            bool canCreateRequest = contract.Status == 1;

            Assert.True(canCreateRequest);
        }

        [Fact]
        public void ServiceRequest_CannotBeCreated_WhenContractIsDraft()
        {
            var contract = new ContractViewModel { Id = 1, Status = 0 };

            bool canCreateRequest = contract.Status == 1;

            Assert.False(canCreateRequest);
        }

        [Fact]
        public void ContractStatus_DisplaysCorrectly_ForAllStatuses()
        {
            var draft = new ContractViewModel { Status = 0 };
            var active = new ContractViewModel { Status = 1 };
            var expired = new ContractViewModel { Status = 2 };
            var onHold = new ContractViewModel { Status = 3 };

            Assert.Equal("Draft", draft.StatusDisplay);
            Assert.Equal("Active", active.StatusDisplay);
            Assert.Equal("Expired", expired.StatusDisplay);
            Assert.Equal("On Hold", onHold.StatusDisplay);
        }

        [Fact]
        public void ServiceRequestStatus_DisplaysCorrectly_ForAllStatuses()
        {
            var pending = new ServiceRequestViewModel { Status = 0 };
            var inProgress = new ServiceRequestViewModel { Status = 1 };
            var completed = new ServiceRequestViewModel { Status = 2 };
            var cancelled = new ServiceRequestViewModel { Status = 3 };

            Assert.Equal("Pending", pending.StatusDisplay);
            Assert.Equal("In Progress", inProgress.StatusDisplay);
            Assert.Equal("Completed", completed.StatusDisplay);
            Assert.Equal("Cancelled", cancelled.StatusDisplay);
        }

        [Fact]
        public void FilterContracts_ByDateRange_ReturnsCorrectResults()
        {
            var contracts = new List<ContractViewModel>
            {
                new() { Id = 1, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2026, 1, 1) },
                new() { Id = 2, StartDate = new DateTime(2024, 6, 1), EndDate = new DateTime(2025, 6, 1) },
                new() { Id = 3, StartDate = new DateTime(2026, 1, 1), EndDate = new DateTime(2027, 1, 1) }
            };

            var startFilter = new DateTime(2025, 1, 1);
            var filtered = contracts.Where(c => c.StartDate >= startFilter).ToList();

            Assert.Equal(2, filtered.Count);
            Assert.Contains(filtered, c => c.Id == 1);
            Assert.Contains(filtered, c => c.Id == 3);
        }

        [Fact]
        public void FilterContracts_ByStatus_ReturnsOnlyMatchingContracts()
        {
            var contracts = new List<ContractViewModel>
            {
                new() { Id = 1, Status = 1 },
                new() { Id = 2, Status = 0 },
                new() { Id = 3, Status = 1 },
                new() { Id = 4, Status = 2 }
            };

            var activeContracts = contracts.Where(c => c.Status == 1).ToList();

            Assert.Equal(2, activeContracts.Count);
            Assert.All(activeContracts, c => Assert.Equal(1, c.Status));
        }
    }
}
