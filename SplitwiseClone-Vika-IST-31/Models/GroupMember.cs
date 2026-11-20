namespace SplitwiseClone.Models;

public class GroupMember
{
    public int GroupId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Group? Group { get; set; }

    public User? User { get; set; }
}