namespace SplitwiseClone.DTOs;

public record CreateGroupDto(string Name, string? Description);

public record AddMemberDto(int UserId);

public record SplitDto(int UserId, decimal PaidShare, decimal OwedShare);

public record CreateExpenseDto(string Description, decimal Amount, List<SplitDto> Splits);

public record CreateSettlementDto(int ToUserId, decimal Amount);

public record UserBalanceDto(int UserId, decimal NetBalance);

public record GroupBalancesDto(int GroupId, List<UserBalanceDto> Balances);