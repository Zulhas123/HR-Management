using HrSystem.Domain.Entities;

namespace HrSystem.Application.Abstractions;

public interface IEmployeeService
{
    Task<IReadOnlyList<Employee>> ListAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee> CreateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee employee, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

