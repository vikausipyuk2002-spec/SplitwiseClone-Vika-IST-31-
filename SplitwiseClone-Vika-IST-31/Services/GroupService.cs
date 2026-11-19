using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Models;
using SplitwiseClone.Repositories;

namespace SplitwiseClone.Services;

public interface IGroupService
{
    Group CreateGroup(CreateGroupDto dto, int currentUserId);
    IEnumerable<Group> GetUserGroups(int currentUserId);
    Group GetGroupById(int groupId, int currentUserId);
    void AddMember(int groupId, int userIdToAdd, int currentUserId);
    Expense AddExpense(int groupId, CreateExpenseDto dto, int currentUserId);
    Expense SettleUp(int groupId, CreateSettlementDto dto, int currentUserId);
}

public class GroupService(IUnitOfWork uow) : IGroupService
{
    public Group CreateGroup(CreateGroupDto dto, int currentUserId)
    {
        var group = new Group
        {
            Name = dto.Name,
            Description = dto.Description
        };

        uow.Groups.Add(group);
        uow.Save();

        // Одразу додаємо автора як учасника групи
        uow.GroupMembers.Add(new GroupMember
        {
            GroupId = group.Id,
            UserId = currentUserId,
            JoinedAt = DateTime.UtcNow
        });
        uow.Save();

        return group;
    }

    public IEnumerable<Group> GetUserGroups(int currentUserId)
    {
        var userGroupIds = uow.GroupMembers.GetAll()
            .Where(gm => gm.UserId == currentUserId)
            .Select(gm => gm.GroupId)
            .ToHashSet();

        return uow.Groups.GetAll()
            .Where(g => userGroupIds.Contains(g.Id))
            .ToList();
    }

    public Group GetGroupById(int groupId, int currentUserId)
    {
        var group = uow.Groups.GetById(groupId)
            ?? throw new GroupNotFoundException(groupId);

        EnsureUserInGroup(groupId, currentUserId);
        return group;
    }

    public void AddMember(int groupId, int userIdToAdd, int currentUserId)
    {
        _ = uow.Groups.GetById(groupId)
            ?? throw new GroupNotFoundException(groupId);

        EnsureUserInGroup(groupId, currentUserId);

        var alreadyMember = uow.GroupMembers.GetAll()
            .Any(gm => gm.GroupId == groupId && gm.UserId == userIdToAdd);

        if (!alreadyMember)
        {
            uow.GroupMembers.Add(new GroupMember
            {
                GroupId = groupId,
                UserId = userIdToAdd,
                JoinedAt = DateTime.UtcNow
            });
            uow.Save();
        }
    }

    public Expense AddExpense(int groupId, CreateExpenseDto dto, int currentUserId)
    {
        _ = uow.Groups.GetById(groupId)
            ?? throw new GroupNotFoundException(groupId);

        EnsureUserInGroup(groupId, currentUserId);

        // Перевіряємо, що всі користувачі в частках є учасниками групи
        var memberIds = uow.GroupMembers.GetAll()
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => gm.UserId)
            .ToHashSet();

        foreach (var split in dto.Splits)
        {
            if (!memberIds.Contains(split.UserId))
                throw new UserNotInGroupException(split.UserId, groupId);
        }

        var expense = new Expense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            GroupId = groupId,
            IsPayment = false,
            Date = DateTime.UtcNow,
            Participants = dto.Splits.Select(s => new ExpenseParticipant
            {
                UserId = s.UserId,
                PaidShare = s.PaidShare,
                OwedShare = s.OwedShare
            }).ToList()
        };

        uow.Expenses.Add(expense);
        uow.Save();

        return expense;
    }

    public Expense SettleUp(int groupId, CreateSettlementDto dto, int currentUserId)
    {
        _ = uow.Groups.GetById(groupId)
            ?? throw new GroupNotFoundException(groupId);

        EnsureUserInGroup(groupId, currentUserId);
        EnsureUserInGroup(groupId, dto.ToUserId);

        // Закриття боргу — це Expense з IsPayment = true
        var settlement = new Expense
        {
            Description = "Settle up",
            Amount = dto.Amount,
            GroupId = groupId,
            IsPayment = true,
            Date = DateTime.UtcNow,
            Participants =
            [
                new ExpenseParticipant { UserId = currentUserId, PaidShare = dto.Amount, OwedShare = 0m },
                new ExpenseParticipant { UserId = dto.ToUserId, PaidShare = 0m, OwedShare = dto.Amount }
            ]
        };

        uow.Expenses.Add(settlement);
        uow.Save();

        return settlement;
    }

    private void EnsureUserInGroup(int groupId, int userId)
    {
        var isMember = uow.GroupMembers.GetAll()
            .Any(gm => gm.GroupId == groupId && gm.UserId == userId);

        if (!isMember)
            throw new UserNotInGroupException(userId, groupId);
    }
}