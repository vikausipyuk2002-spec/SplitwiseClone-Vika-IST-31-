namespace SplitwiseClone.Models;

public class Payment
{
    public int Id { get; set; }

    public int FromUserId { get; set; }

    public int ToUserId { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public string? Note { get; set; }

    public User? FromUser { get; set; }

    public User? ToUser { get; set; }
}