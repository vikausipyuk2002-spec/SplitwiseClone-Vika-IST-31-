namespace SplitwiseClone.Models;

public class Expense
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public int GroupId { get; set; }

    public int PaidByUserId { get; set; }

    public Group? Group { get; set; }

    public User? PaidByUser { get; set; }
}