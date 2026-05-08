using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HrSystem.Web.Controllers;

public sealed class OvertimeRequestsController(
    ICrudService<OvertimeRequest> overtimeRequests,
    IOvertimeService overtime,
    ILeaveCalendarService calendar) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await overtimeRequests.ListAsync(cancellationToken);
        return View(items);
    }

    public IActionResult Create() => View(new OvertimeRequestFormVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OvertimeRequestFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var policy = await overtime.GetActivePolicyAsync(cancellationToken);
        var isHoliday = await calendar.IsHolidayAsync(DateOnly.FromDateTime(vm.Date), cancellationToken);

        await overtimeRequests.CreateAsync(new OvertimeRequest
        {
            EmployeeId = vm.EmployeeId,
            Date = DateOnly.FromDateTime(vm.Date),
            AttendanceRecordId = vm.AttendanceRecordId,
            IsHoliday = isHoliday,
            PayMultiplier = isHoliday ? policy.HolidayMultiplier : policy.NormalMultiplier,
            CalculatedMinutes = Math.Max(0, vm.CalculatedMinutes),
            RequestedMinutes = Math.Max(0, vm.RequestedMinutes),
            ApprovalLevelsRequired = Math.Max(1, policy.ApprovalLevelsRequired),
            ApprovalLevelsApproved = 0,
            Status = OvertimeRequestStatus.Pending,
            Reason = vm.Reason
        }, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await overtimeRequests.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != OvertimeRequestStatus.Pending || entity.ApprovalLevelsApproved > 0)
        {
            return BadRequest("Only pending requests with no approvals can be edited.");
        }

        return View(new OvertimeRequestFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Date = entity.Date.ToDateTime(TimeOnly.MinValue),
            RequestedMinutes = entity.RequestedMinutes,
            CalculatedMinutes = entity.CalculatedMinutes,
            AttendanceRecordId = entity.AttendanceRecordId,
            Reason = entity.Reason
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OvertimeRequestFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var entity = await overtimeRequests.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        if (entity.Status != OvertimeRequestStatus.Pending || entity.ApprovalLevelsApproved > 0)
        {
            return BadRequest("Only pending requests with no approvals can be edited.");
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.Date = DateOnly.FromDateTime(vm.Date);
        entity.AttendanceRecordId = vm.AttendanceRecordId;
        entity.CalculatedMinutes = Math.Max(0, vm.CalculatedMinutes);
        entity.RequestedMinutes = Math.Max(0, vm.RequestedMinutes);
        entity.Reason = vm.Reason;

        var policy = await overtime.GetActivePolicyAsync(cancellationToken);
        entity.IsHoliday = await calendar.IsHolidayAsync(entity.Date, cancellationToken);
        entity.PayMultiplier = entity.IsHoliday ? policy.HolidayMultiplier : policy.NormalMultiplier;

        await overtimeRequests.UpdateAsync(entity, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await overtimeRequests.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await overtimeRequests.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? note, CancellationToken cancellationToken)
    {
        await overtime.ApproveAsync(id, User?.Identity?.Name ?? "system", note, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string? note, CancellationToken cancellationToken)
    {
        await overtime.RejectAsync(id, User?.Identity?.Name ?? "system", note, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult AutoGenerate() => View(new OvertimeAutoGenerateVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoGenerate(OvertimeAutoGenerateVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        await overtime.AutoGenerateFromAttendanceAsync(
            DateOnly.FromDateTime(vm.FromInclusive),
            DateOnly.FromDateTime(vm.ToInclusive),
            vm.CreateIfMissing,
            cancellationToken);

        return RedirectToAction(nameof(Index));
    }
}
