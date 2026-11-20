using SplitwiseClone.Data;
using SplitwiseClone.Models;

namespace SplitwiseClone.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DB _context;

    public IRepository<User> Users { get; private set; }
    public IRepository<Expense> Expenses { get; private set; }
    public IRepository<Group> Groups { get; private set; }
    public IRepository<GroupMember> GroupMembers { get; private set; }
    public IRepository<ExpenseParticipant> ExpenseParticipants { get; private set; }

    public UnitOfWork(DB context)
    {
        _context = context;
        Users = new Repository<User>(_context);
        Expenses = new Repository<Expense>(_context);
        Groups = new Repository<Group>(_context);
        GroupMembers = new Repository<GroupMember>(_context);
        ExpenseParticipants = new Repository<ExpenseParticipant>(_context);
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}