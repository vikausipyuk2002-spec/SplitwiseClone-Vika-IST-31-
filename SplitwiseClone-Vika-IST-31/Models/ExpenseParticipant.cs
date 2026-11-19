namespace SplitwiseClone.Models;

public class ExpenseParticipant
{
    public int Id { get; set; }

    public int ExpenseId { get; set; }

    public int UserId { get; set; }

    public decimal PaidShare { get; set; }

    public decimal OwedShare { get; set; }

    public Expense? Expense { get; set; }

    public User? User { get; set; }
}