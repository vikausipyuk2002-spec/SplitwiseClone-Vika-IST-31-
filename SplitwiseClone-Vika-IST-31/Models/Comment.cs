namespace SplitwiseClone.Models;

public class Comment
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int ExpenseId { get; set; }

    public int UserId { get; set; }

    public Expense? Expense { get; set; }

    public User? User { get; set; }
}