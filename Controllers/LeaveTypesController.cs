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
		public async Task<ActionResult> Index()
		{
			var leaveTypes = await _repo.FindAll();
			var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>((List<LeaveType>)leaveTypes);
			return View(model);
		}

		// GET: LeaveTypesController/Details/5
		public async Task<ActionResult> Details(int id)
		{
			var isExists = await _repo.IsExists(id);
			if (!isExists) {
				return NotFound();
			}
			var leaveType = await _repo.FindById(id);
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
		public async Task<ActionResult> Create(LeaveTypeVM model)
		{
			try
			{
				if (!ModelState.IsValid) {
					return View(model);
				}

				var leaveType = _mapper.Map<LeaveType>(model);
				leaveType.DateCreated = DateTime.Now;

				var isSuccess = await _repo.Create(leaveType);

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
		public async Task<ActionResult> Edit(int id)
		{
			var isExists = await _repo.IsExists(id);
			if (!isExists) {
				return NotFound();
			}
			var leaveType = _repo.FindById(id);
			var model = _mapper.Map<LeaveTypeVM>(leaveType);
			return View(model);
		}

		// POST: LeaveTypesController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(LeaveTypeVM model)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(model);
				}

				var leaveType = _mapper.Map<LeaveType>(model);
				var isSuccess = await _repo.Update(leaveType);

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
		public async Task<ActionResult> Delete(int id)
		{
			var leaveType = await _repo.FindById(id);

			if (leaveType == null)
			{
				return NotFound();
			}

			var isSuccess = await _repo.Delete(leaveType);

			if (!isSuccess)
			{
				return BadRequest();
			}
			return RedirectToAction(nameof(Index));
		}

		// POST: LeaveTypesController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete(int id, LeaveTypeVM model)
		{
			try
			{
				var leaveType = await _repo.FindById(id);

				if (leaveType == null) {
					return NotFound();
				}

				var isSuccess = await _repo.Delete(leaveType);

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
