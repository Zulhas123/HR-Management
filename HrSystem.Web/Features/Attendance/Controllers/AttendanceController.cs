using HrSystem.Application.Abstractions;
using HrSystem.Application.Attendance;
using HrSystem.Domain.Entities;
using HrSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HrSystem.Web.Controllers;

public sealed class AttendanceController(
    ICrudService<AttendanceRecord> attendance,
    ICrudService<Employee> employees,
    ICrudService<Shift> shifts) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var items = await attendance.ListAsync(cancellationToken);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var vm = new AttendanceRecordFormVm();
        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AttendanceRecordFormVm vm, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var record = new AttendanceRecord
        {
            EmployeeId = vm.EmployeeId,
            Date = DateOnly.FromDateTime(vm.Date),
            ShiftId = vm.ShiftId,
            CheckInTime = vm.CheckInTime,
            CheckOutTime = vm.CheckOutTime,
            Source = vm.Source,
            DeviceVendor = vm.DeviceVendor,
            DeviceId = vm.DeviceId,
            DeviceUserId = vm.DeviceUserId,
            RfidCardId = vm.RfidCardId,
            MobileDeviceId = vm.MobileDeviceId,
            Latitude = vm.Latitude,
            Longitude = vm.Longitude,
            LocationAccuracyMeters = vm.LocationAccuracyMeters,
            Notes = vm.Notes
        };

        AttendanceMetrics.ApplyMissingPunchStatus(record);

        if (record.ShiftId.HasValue)
        {
            var shift = await shifts.GetByIdAsync(record.ShiftId.Value, cancellationToken);
            if (shift is not null)
            {
                AttendanceMetrics.ApplyDerivedMetrics(record, shift);
            }
        }

        await attendance.CreateAsync(record, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var entity = await attendance.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var vm = new AttendanceRecordFormVm
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            Date = entity.Date.ToDateTime(TimeOnly.MinValue),
            ShiftId = entity.ShiftId,
            CheckInTime = entity.CheckInTime,
            CheckOutTime = entity.CheckOutTime,
            Source = entity.Source,
            DeviceVendor = entity.DeviceVendor,
            DeviceId = entity.DeviceId,
            DeviceUserId = entity.DeviceUserId,
            RfidCardId = entity.RfidCardId,
            MobileDeviceId = entity.MobileDeviceId,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude,
            LocationAccuracyMeters = entity.LocationAccuracyMeters,
            Notes = entity.Notes
        };

        await PopulateLookupsAsync(vm, cancellationToken);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AttendanceRecordFormVm vm, CancellationToken cancellationToken)
    {
        if (id != vm.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateLookupsAsync(vm, cancellationToken);
            return View(vm);
        }

        var entity = await attendance.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        entity.EmployeeId = vm.EmployeeId;
        entity.Date = DateOnly.FromDateTime(vm.Date);
        entity.ShiftId = vm.ShiftId;
        entity.CheckInTime = vm.CheckInTime;
        entity.CheckOutTime = vm.CheckOutTime;
        entity.Source = vm.Source;
        entity.DeviceVendor = vm.DeviceVendor;
        entity.DeviceId = vm.DeviceId;
        entity.DeviceUserId = vm.DeviceUserId;
        entity.RfidCardId = vm.RfidCardId;
        entity.MobileDeviceId = vm.MobileDeviceId;
        entity.Latitude = vm.Latitude;
        entity.Longitude = vm.Longitude;
        entity.LocationAccuracyMeters = vm.LocationAccuracyMeters;
        entity.Notes = vm.Notes;

        AttendanceMetrics.ApplyMissingPunchStatus(entity);

        if (entity.ShiftId.HasValue)
        {
            var shift = await shifts.GetByIdAsync(entity.ShiftId.Value, cancellationToken);
            if (shift is not null)
            {
                AttendanceMetrics.ApplyDerivedMetrics(entity, shift);
            }
        }

        await attendance.UpdateAsync(entity, cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await attendance.GetByIdAsync(id, cancellationToken);
        return entity is null ? NotFound() : View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        await attendance.DeleteAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookupsAsync(AttendanceRecordFormVm vm, CancellationToken cancellationToken)
    {
        vm.Employees = (await employees.ListAsync(cancellationToken))
            .Select(e => new SelectListItem($"{e.EmployeeCode} - {e.FirstName} {e.LastName}", e.Id.ToString()))
            .ToList();

        vm.Shifts = (await shifts.ListAsync(cancellationToken))
            .Select(s => new SelectListItem($"{s.Name} ({s.StartTime}-{s.EndTime})", s.Id.ToString()))
            .ToList();
    }
}
