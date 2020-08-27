using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Repository
{
	public class LeaveRequestRepository : ILeaveRequestRepository
	{

		private readonly ApplicationDbContext _db;

		public LeaveRequestRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<bool> Create(LeaveRequest entity)
		{
			_db.LeaveRequests.Add(entity);
			return await Save();
		}

		public async Task<bool> Delete(LeaveRequest entity)
		{
			_db.LeaveRequests.Remove(entity);
			return await Save();
		}

		public async Task<ICollection<LeaveRequest>> FindAll()
		{
			return await _db.LeaveRequests
				.Include(q => q.RequestingEmployee)
				.Include(q => q.ApprovedBy)
				.Include(q => q.LeaveType)
				.ToListAsync();
		}

		public async Task<LeaveRequest> FindById(int id)
		{
			var leaveRequests = await FindAll();
			return leaveRequests.FirstOrDefault(q => q.RequestingEmployeeId == id.ToString());
		}

		public async Task<ICollection<LeaveRequest>> FindByEmployeeId(string id)
		{
			var leaveRequests = await FindAll();
			return leaveRequests
				.Where(q => q.RequestingEmployeeId == id.ToString())
				.ToList();
		}

		public async Task<bool> IsExists(int id)
		{
			return await _db.LeaveTypes.AnyAsync(q => q.Id == id);

		}

		public async Task<bool> Save()
		{
			return await _db.SaveChangesAsync() > 0;
		}

		public async Task<bool> Update(LeaveRequest entity)
		{
			_db.LeaveRequests.Update(entity);
			return await Save();
		}
	}
}
