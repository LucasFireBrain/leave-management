using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
	public class LeaveAllocationRepository : ILeaveAllocationRepository
	{

		private readonly ApplicationDbContext _db;

		public LeaveAllocationRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
		{
			var period = DateTime.Now.Year;
			var allocations = await FindAll();
			return allocations.Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period).Any();
		}

		public async Task<bool >Create(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Add(entity);
			return await Save();
		}

		public async Task<bool> Delete(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Remove(entity);
			return await Save();
		}

		public async Task<ICollection<LeaveAllocation>> FindAll()
		{
			return await _db.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q => q.Employee)
				.ToListAsync();
		}

		public async Task<LeaveAllocation> FindById(int id)
		{
			return await _db.LeaveAllocations
				.Include(q => q.Employee)
				.Include(q => q.LeaveType)
				.FirstOrDefaultAsync(item => item.Id == id);
		}

		public async Task<ICollection<LeaveAllocation>> FindByEmployeeId(string id)
		{
			var allocations = await FindAll();
			return allocations
				.Where(q => q.EmployeeId == id)
				.ToList();
		}

		public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string id, int leaveTypeId)
		{
			var allocations = await FindAll();
			return allocations.FirstOrDefault(q => q.Employee.Id == id && q.LeaveTypeId == leaveTypeId);
		}

		public async Task<bool> IsExists(int id)
		{
			return await _db.LeaveTypes.AnyAsync(q => q.Id == id);

		}

		public async Task<bool> Save()
		{
			return await _db.SaveChangesAsync() > 0;
		}

		public async Task<bool> Update(LeaveAllocation entity)
		{
			_db.LeaveAllocations.Update(entity);
			return await Save();
		}
	}
}
