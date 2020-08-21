  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_management.Contracts;
using leave_management.Data;
using leave_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{

	[Authorize(Roles = "Administrator")]    // If not logged in, redirect to login page 

	public class LeaveAllocationController : Controller
	{

		private readonly ILeaveTypeRepository _leaveRepo;
		private readonly ILeaveAllocationRepository _leaveAllocationRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<Employee> _userManager;

		public LeaveAllocationController(ILeaveTypeRepository leaveRepo, ILeaveAllocationRepository leaveAllocationRepo, IMapper mapper, UserManager<Employee> userManager)
		{
			_leaveRepo = leaveRepo;
			_leaveAllocationRepo = leaveAllocationRepo;
			_mapper = mapper;
			_userManager = userManager;
		}

		// GET: LeaveAllocationController
		public ActionResult Index()
		{
			var leaveTypes = _leaveRepo.FindAll();
			var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>((List<LeaveType>)leaveTypes);
			var model = new CreateLeaveAllocationVM
			{
				LeaveTypes = mappedLeaveTypes,
				NumberUpdated = 0
			};
			return View(model);

		}


		public ActionResult SetLeave(int id) {
			var leaveType = _leaveRepo.FindById(id);
			var employees = _userManager.GetUsersInRoleAsync("Employee").Result;

			int counter = 0;
			foreach (var e in employees)
			{
				if (_leaveAllocationRepo.CheckAllocation(id, e.Id)) {
					continue;
				}
				var allocation = new LeaveAllocationVM
				{
					DateCreated = DateTime.Now,
					EmployeeId = e.Id,
					LeaveTypeId = id,
					NumberOfDays = leaveType.DefaultDays,
					Period = DateTime.Now.Year
				};
				var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
				_leaveAllocationRepo.Create(leaveAllocation);
				counter++;
			}
			
			return RedirectToAction(nameof(Index));
		}

		public ActionResult ListEmployees() {
			var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
			var model = _mapper.Map<List<EmployeeVM>>(employees);
			return View(model);
		}


		// GET: LeaveAllocationController/Details/5
		public ActionResult Details(string id)
		{
			//Because is Async, we get result
			var employee = _mapper.Map<EmployeeVM>(_userManager.FindByIdAsync(id).Result);
			var allocations = _mapper.Map<List<LeaveAllocationVM>>(_leaveAllocationRepo.FindByEmployeeId(id));
			var model = new ViewAllocationsVM {
				Employee = employee,
				Allocations = allocations
			};
			return View(model);
		}

		// GET: LeaveAllocationController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: LeaveAllocationController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: LeaveAllocationController/Edit/5
		public ActionResult Edit(int id)
		{
			var leaveAllocation = _leaveAllocationRepo.FindById(id);
			var model = _mapper.Map<EditLeaveAllocationVM>(leaveAllocation);
			return View(model);
		}

		// POST: LeaveAllocationController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditLeaveAllocationVM model)
		{
			try
			{
				if (!ModelState.IsValid) {
					return View(model);
				}
				var record = _leaveAllocationRepo.FindById(model.Id);
				record.NumberOfDays = model.NumberOfDays;
				var isSuccess = _leaveAllocationRepo.Update(record);
				if (!isSuccess) {
					ModelState.AddModelError("", "Error while saving edit");
					return View(model);
				}

				return RedirectToAction(nameof(Details), new {id = model.EmployeeId });
			}
			catch
			{
				return View(model);
			}
		}

		// GET: LeaveAllocationController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: LeaveAllocationController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
