using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Data;

namespace SplitwiseClone.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DB _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DB context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IEnumerable<T> GetAll() => _dbSet.ToList();

    public T GetById(int id) => _dbSet.Find(id);

    public void Add(T entity) => _dbSet.Add(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);
}