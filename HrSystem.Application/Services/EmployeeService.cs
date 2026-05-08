using HrSystem.Application.Abstractions;
using HrSystem.Domain.Entities;
using HrSystem.Domain.Repositories;

namespace HrSystem.Application.Services;

public sealed class EmployeeService(IRepository<Employee> repository) : IEmployeeService
{
    public Task<IReadOnlyList<Employee>> ListAsync(CancellationToken cancellationToken = default) =>
        repository.ListAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        employee.EmployeeCode = $"TMP-{Guid.NewGuid().ToString("N")[..16]}";

        await repository.AddAsync(employee, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        employee.EmployeeCode = $"EMP-{employee.Id:D6}";
        await repository.UpdateAsync(employee, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return employee;
    }

    public async Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await repository.UpdateAsync(employee, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        await repository.DeleteAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
