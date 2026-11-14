using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Models;
using System.Linq;

namespace SplitwiseClone.Data;

public class DB : IdentityDbContext<IdentityUser>
{
    // Доданий конструктор для підключення бази з Program.cs (Neon)
    public DB(DbContextOptions<DB> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Обов'язково для Identity

        // Вимикаємо каскадне видалення, щоб уникнути конфліктів ключів
        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}