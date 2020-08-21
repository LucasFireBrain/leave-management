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

		public bool Create(LeaveRequest entity)
		{
			_db.LeaveRequests.Add(entity);
			return Save();
		}

		public bool Delete(LeaveRequest entity)
		{
			_db.LeaveRequests.Remove(entity);
			return Save();
		}

		public ICollection<LeaveRequest> FindAll()
		{
			return _db.LeaveRequests
				.Include(q => q.RequestingEmployee)
				.Include(q => q.ApprovedBy)
				.Include(q => q.LeaveType)
				.ToList();
		}

		public LeaveRequest FindById(int id)
		{
			return FindAll()
				.FirstOrDefault(item => item.Id == id);
		}

		public ICollection<LeaveRequest> FindByEmployeeId(string id)
		{
			return FindAll()
				.Where(q => q.RequestingEmployeeId == id.ToString())
				.ToList();
		}

		public bool IsExists(int id)
		{
			return _db.LeaveTypes.Any(q => q.Id == id);

		}

		public bool Save()
		{
			return _db.SaveChanges() > 0;
		}

		public bool Update(LeaveRequest entity)
		{
			_db.LeaveRequests.Update(entity);
			return Save();
		}
	}
}
