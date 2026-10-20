using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Models;

namespace SplitwiseClone.Data;

public class DB : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // тестовий рядок підключення для компіляції
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SplitwiseDb;Trusted_Connection=True;");
    }
}