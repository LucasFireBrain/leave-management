using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Models
{
	public class LeaveAllocationVM
	{
		[Key]
		public int Id { get; set; }

		[Display(Name = "Number of Days")]
		public int NumberOfDays { get; set; }
		[Display(Name = "Date Created")]
		public DateTime DateCreated { get; set; }

		public int Period { get; set; }

		[ForeignKey("EmployeeId")] 
		public EmployeeVM Employee { get; set; }
		[Display(Name = "ID of Employee")]
		public string EmployeeId { get; set; }

		[ForeignKey("LeaveTypeId")]
		public LeaveTypeVM LeaveType { get; set; }
		[Display(Name = "Leave Type ID")]
		public int LeaveTypeId { get; set; }

		public IEnumerable<SelectListItem> Employees { get; set; }
		public IEnumerable<SelectListItem> LeaveTypes { get; set; }
		
	}

	public class CreateLeaveAllocationVM {
		public int NumberUpdated { get; set; }
		public List<LeaveTypeVM> LeaveTypes { get; set; }
	}

	public class EditLeaveAllocationVM {
		public int Id { get; set; }
		public LeaveTypeVM LeaveType { get; set; }
		public int NumberOfDays { get; set; }
		public EmployeeVM Employee { get; set; }
		public string EmployeeId { get; set; }

	}
	public class ViewAllocationsVM {
		public EmployeeVM Employee { get; set; }
		public List<LeaveAllocationVM> Allocations { get; set; }
	}
}
