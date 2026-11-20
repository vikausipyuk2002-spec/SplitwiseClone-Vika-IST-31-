using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SplitwiseClone.Models;

namespace SplitwiseClone.Data;

public class DB : IdentityDbContext<IdentityUser>
{
    public DB(DbContextOptions<DB> options) : base(options)
    {
    }

    public new DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseParticipant> ExpenseParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Складений ключ для GroupMember (без зайвого Id)
        builder.Entity<GroupMember>()
            .HasKey(gm => new { gm.GroupId, gm.UserId });

        // Точність для грошей (decimal 18, 2)
        builder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);

        builder.Entity<ExpenseParticipant>()
            .Property(p => p.PaidShare)
            .HasPrecision(18, 2);

        builder.Entity<ExpenseParticipant>()
            .Property(p => p.OwedShare)
            .HasPrecision(18, 2);

        // Вимикаємо каскадне видалення, щоб уникнути конфліктів ключів
        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}