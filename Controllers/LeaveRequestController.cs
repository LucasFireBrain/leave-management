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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Controllers
{
	[Authorize]
	public class LeaveRequestController : Controller
	{
		private readonly ILeaveRequestRepository _leaveRequestRepo;
		private readonly ILeaveTypeRepository _leaveTypeRepo;
		private readonly ILeaveAllocationRepository _leaveAllocRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<Employee> _userManager;

		public LeaveRequestController(ILeaveRequestRepository leaveRequestRepo, ILeaveTypeRepository leaveTypeRepo, ILeaveAllocationRepository leaveAllocRepo, IMapper mapper, UserManager<Employee> userManager) {
			_leaveRequestRepo = leaveRequestRepo;
			_leaveTypeRepo = leaveTypeRepo;
			_leaveAllocRepo = leaveAllocRepo;
			_mapper = mapper;
			_userManager = userManager;
		}

		[Authorize(Roles = "Administrator")]
		// GET: LeaveRequestController
		public async Task<ActionResult> Index()
		{
			var leaveRequests = await _leaveRequestRepo.FindAll();
			var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
			var model = new AdminLeaveRequestViewVM
			{
				TotalRequests = leaveRequests.Count,
				ApprovedRequests = leaveRequestsModel.Count(q => q.Approved == true),
				PendingRequests = leaveRequestsModel.Count(q => q.Approved == null),
				RejectedRequests = leaveRequestsModel.Count(q => q.Approved == false),
				LeaveRequests = leaveRequestsModel
			};

			return View(model);
		}

		// GET: LeaveRequestController/Details/5
		public async Task<ActionResult> Details(int id)
		{

			var leaveRequest = await _leaveRequestRepo.FindById(id);
			var model = _mapper.Map<LeaveRequestVM>(leaveRequest);

			return View(model);
		}

		public async Task<ActionResult> ApproveRequest(int id) {

			try
			{
				var user = await _userManager.GetUserAsync(User);
				var leaveRequest = await _leaveRequestRepo.FindById(id);
				var employeeId = leaveRequest.RequestingEmployeeId;
				var leaveTypeId = leaveRequest.LeaveTypeId;
				var allocation = await _leaveAllocRepo.GetLeaveAllocationsByEmployeeAndType(employeeId, leaveTypeId);

				//Substract days from allocation
				var numberOfDays = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
				allocation.NumberOfDays -= numberOfDays;

				leaveRequest.Approved = true;
				leaveRequest.ApprovedById = user.Id;
				leaveRequest.DateActioned = DateTime.Now;

				//Save
				await _leaveRequestRepo.Update(leaveRequest);
				await _leaveAllocRepo.Update(allocation); 
				return RedirectToAction(nameof(Index));
				
			}
			catch (Exception e)
			{
				return RedirectToAction(nameof(Index));
			}

			
		}

		public async Task<ActionResult> RejectRequest(int id)
		{
			try
			{
				var leaveRequest = await _leaveRequestRepo.FindById(id);
				leaveRequest.Approved = false;
				var user = await _userManager.GetUserAsync(User);
				leaveRequest.ApprovedById = user.Id;
				leaveRequest.DateActioned = DateTime.Now;

				//Save
				await _leaveRequestRepo.Update(leaveRequest);
				return RedirectToAction(nameof(Index));

			}
			catch (Exception e)
			{
				return RedirectToAction(nameof(Index));
			}
		}

		// GET: LeaveRequestController/Create
		public async Task<ActionResult> Create()
		{
			var leaveTypes = await _leaveTypeRepo.FindAll();
			var leaveTypeItems = leaveTypes.Select(q => new SelectListItem { 
				Text = q.Name,
				Value = q.Id.ToString()
			});
			var model = new CreateLeaveRequestVM
			{
				LeaveTypes = leaveTypeItems
			};
			return View(model);
		}

		// POST: LeaveRequestController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(CreateLeaveRequestVM model)
		{
			try 
			{

				var startDate = Convert.ToDateTime(model.StartDate);
				var endDate = Convert.ToDateTime(model.EndDate);

				var leaveTypes = await _leaveTypeRepo.FindAll();
				var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
				{
					Text = q.Name,
					Value = q.Id.ToString()
				});

				model.LeaveTypes = leaveTypeItems;

				if (!ModelState.IsValid) {
					return View(model);
				}

				if (DateTime.Compare(startDate, endDate) > 0) {
					ModelState.AddModelError("", "Invalid Dates");
					return View(model);
				}

				//Getting logged in user
				var employee = await _userManager.GetUserAsync(User);
				var allocation = await _leaveAllocRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id.ToString(), model.LeaveTypeId);
				int daysRequested = (int)(endDate - startDate).TotalDays;

				if (allocation == null) { 
					
				}

				if (daysRequested > allocation.NumberOfDays) {
					ModelState.AddModelError("", "you do not have enough days for this request");
					return View(model);
				}

				var leaveRequestModel = new LeaveRequestVM
				{
					RequestingEmployeeId = employee.Id,
					StartDate = startDate,
					EndDate = endDate,
					Approved = null,
					DateRequested = DateTime.Now,
					DateActioned = null,
					LeaveTypeId = model.LeaveTypeId
				};

				var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
				var isSuccess = await _leaveRequestRepo.Create(leaveRequest);

				if (!isSuccess) {
					ModelState.AddModelError("", "Something went wrong when submitting your application");
					return View(model);
				}

				return RedirectToAction("MyLeaves");
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(model);
			}
		}

		
		public async Task<ActionResult> MyLeaves(int id)
		{
			var employee = await _userManager.GetUserAsync(User);
			var employeeVM = _mapper.Map<EmployeeVM>(employee);
			
			var leaveRequests = await _leaveRequestRepo.FindByEmployeeId(employee.Id);
			var leaveRequestsVM = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
			
			var leaveAllocations = await _leaveAllocRepo.FindByEmployeeId(employee.Id);
			var leaveAllocationsVM = _mapper.Map<List<LeaveAllocationVM>>(leaveAllocations);

			var model = new EmployeeLeaveRequestsVM
			{
				RequestingEmployee = employeeVM,
				RequestingEmployeeId = employee.Id,
				LeaveRequests = leaveRequestsVM,
				LeaveAllocations = leaveAllocationsVM
			};

			
			return View(model);
		}

		// GET: LeaveRequestController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: LeaveRequestController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
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

		// GET: LeaveRequestController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: LeaveRequestController/Delete/5
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
