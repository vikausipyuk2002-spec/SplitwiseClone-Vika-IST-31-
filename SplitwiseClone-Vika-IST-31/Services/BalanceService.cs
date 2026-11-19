using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Repositories;

namespace SplitwiseClone.Services;

public interface IBalanceService
{
    GroupBalancesDto GetGroupBalances(int groupId, int currentUserId);
}

public class BalanceService(IUnitOfWork uow) : IBalanceService
{
    public GroupBalancesDto GetGroupBalances(int groupId, int currentUserId)
    {
        _ = uow.Groups.GetById(groupId)
            ?? throw new GroupNotFoundException(groupId);

        var memberIds = uow.GroupMembers.GetAll()
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => gm.UserId)
            .ToList();

        if (!memberIds.Contains(currentUserId))
            throw new UserNotInGroupException(currentUserId, groupId);

        var groupExpenseIds = uow.Expenses.GetAll()
            .Where(e => e.GroupId == groupId)
            .Select(e => e.Id)
            .ToHashSet();

        var participants = uow.ExpenseParticipants.GetAll()
            .Where(p => groupExpenseIds.Contains(p.ExpenseId))
            .ToList();

        // Одна формула рахує і витрати, і повернення боргів (IsPayment = true)
        var balances = memberIds.Select(userId =>
        {
            var net = participants
                .Where(p => p.UserId == userId)
                .Sum(p => p.PaidShare - p.OwedShare);

            return new UserBalanceDto(userId, net);
        }).ToList();

        return new GroupBalancesDto(groupId, balances);
    }
}