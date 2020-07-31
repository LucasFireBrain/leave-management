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
using Microsoft.AspNetCore.Mvc;

namespace leave_management.Controllers
{
	[Authorize(Roles = "Administrator")]	// If not logged in, redirect to login page 
	public class LeaveTypesController : Controller
	{

		private readonly ILeaveTypeRepository _repo;
		private readonly IMapper _mapper;

		public LeaveTypesController(ILeaveTypeRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;  
		}
	
		// GET: LeaveTypesController
		public ActionResult Index()
		{
			var leaveTypes = _repo.FindAll();
			var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>((List<LeaveType>)leaveTypes);
			return View(model);
		}

		// GET: LeaveTypesController/Details/5
		public ActionResult Details(int id)
		{
			if (!_repo.IsExists(id)) {
				return NotFound();
			}
			var leaveType = _repo.FindById(id);
			var model = _mapper.Map<LeaveTypeVM>(leaveType);
			return View(model); 
		}

		// GET: LeaveTypesController/Create
		public ActionResult Create()
		{
			
			return View();
		}

		// POST: LeaveTypesController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(LeaveTypeVM model)
		{
			try
			{
				if (!ModelState.IsValid) {
					return View(model);
				}

				var leaveType = _mapper.Map<LeaveType>(model);
				leaveType.DateCreated = DateTime.Now;

				var isSuccess = _repo.Create(leaveType);

				if (!isSuccess) {
					ModelState.AddModelError("", "Something went wrong when Creating...");
					return View(model);
				}

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				ModelState.AddModelError("", "Something went wrong when Creating...");
				return View(model);
			}
		}

		// GET: LeaveTypesController/Edit/5
		public ActionResult Edit(int id)
		{
			if (!_repo.IsExists(id)) {
				return NotFound();
			}
			var leaveType = _repo.FindById(id);
			var model = _mapper.Map<LeaveTypeVM>(leaveType);
			return View(model);
		}

		// POST: LeaveTypesController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(LeaveTypeVM model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}

				var leaveType = _mapper.Map<LeaveType>(model);
				var isSuccess = _repo.Update(leaveType);

				if (!isSuccess) {
					ModelState.AddModelError("", "Something went wrong on Edit");
					return View(model);
				}

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				ModelState.AddModelError("", "Something went wrong on Edit");
				return View(model);
			}
		}

		// GET: LeaveTypesController/Delete/5
		public ActionResult Delete(int id)
		{
			var leaveType = _repo.FindById(id);

			if (leaveType == null)
			{
				return NotFound();
			}

			var isSuccess = _repo.Delete(leaveType);

			if (!isSuccess)
			{
				return BadRequest();
			}
			return RedirectToAction(nameof(Index));
		}

		// POST: LeaveTypesController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, LeaveTypeVM model)
		{
			try
			{
				var leaveType = _repo.FindById(id);

				if (leaveType == null) {
					return NotFound();
				}

				var isSuccess = _repo.Delete(leaveType);

				if (!isSuccess) {
					return View(model);
				}
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
