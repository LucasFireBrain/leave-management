using leave_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Contracts
{
	public interface ILeaveAllocationRepository : IRepositoryBase<LeaveAllocation>
	{
		Task <bool> CheckAllocation(int leaveTypeId, string employeeId);
		Task<ICollection<LeaveAllocation>> FindByEmployeeId(string id);
		Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeId, int leaveTypeId);

	}
}
