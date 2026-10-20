using SplitwiseClone.Models;

namespace SplitwiseClone.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Expense> Expenses { get; }
    IRepository<Group> Groups { get; }

    void Save();
}