using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
	public class LeaveRequestVM
	{
		public int Id { get; set; }
		public EmployeeVM RequestingEmployee { get; set; }
		public string RequestingEmployeeId { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public LeaveTypeVM LeaveType { get; set; }
		public int LeaveTypeId { get; set; }
		public DateTime DateRequested { get; set; }
		public DateTime? DateActioned { get; set; }

		[Display(Name = "Employee Comments")]
		[MaxLength(300)]
		public string RequestComment { get; set; }

		public bool? Approved { get; set; }
		public EmployeeVM ApprovedBy { get; set; }
		public string ApprovedById { get; set; }

	}

	public class AdminLeaveRequestViewVM {
		[Display(Name = "Total Number of Requests")]
		public int TotalRequests { get; set; }
		[Display(Name = "Approved Requests")]
		public int ApprovedRequests { get; set; }
		[Display(Name = "Pending Requests")]
		public int PendingRequests { get; set; }
		[Display(Name = "Rejected Requests")]
		public int RejectedRequests { get; set; }
		
		public List<LeaveRequestVM> LeaveRequests { get; set; }
	}

	public class CreateLeaveRequestVM { 
		[Display(Name = "Start Date")]
		[Required]
		public string StartDate { get; set; }
		
		[Display(Name = "End Date")]
		[Required]
		public string EndDate { get; set; }

		public IEnumerable<SelectListItem> LeaveTypes { get; set; }

		[Display(Name = "Leave Type")]
		public int LeaveTypeId { get; set; }

		[Display(Name = "Employee Comments")]
		[MaxLength(300)]
		public string RequestComment { get; set; }

	}

	public class EmployeeLeaveRequestsVM {
		public EmployeeVM RequestingEmployee { get; set; }
		public string RequestingEmployeeId { get; set; }

		public List<LeaveRequestVM> LeaveRequests { get; set; }

		public List<LeaveAllocationVM> LeaveAllocations { get; set; }
	}
}
