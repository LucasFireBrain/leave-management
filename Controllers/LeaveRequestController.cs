﻿using System;
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
		public ActionResult Index()
		{
			var leaveRequests = _leaveRequestRepo.FindAll();
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
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: LeaveRequestController/Create
		public ActionResult Create()
		{
			var leaveTypes = _leaveTypeRepo.FindAll();
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
		public ActionResult Create(CreateLeaveRequestVM model)
		{
			try 
			{

				var startDate = Convert.ToDateTime(model.StartDate);
				var endDate = Convert.ToDateTime(model.EndDate);

				var leaveTypes = _leaveTypeRepo.FindAll();
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
				var employee = _userManager.GetUserAsync(User).Result;
				var allocation = _leaveAllocRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id.ToString(), model.LeaveTypeId);
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
				var isSuccess = _leaveRequestRepo.Create(leaveRequest);

				if (!isSuccess) {
					ModelState.AddModelError("", "Something went wrong when submitting your application");
					return View(model);
				}

				return RedirectToAction(nameof(Index),"Home");
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(model);
			}
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
