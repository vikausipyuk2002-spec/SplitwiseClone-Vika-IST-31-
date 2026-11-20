using Moq;
using SplitwiseClone.DTOs;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Models;
using SplitwiseClone.Repositories;
using SplitwiseClone.Services;
using Xunit;

namespace UnitTest1;

public class GroupAndBalanceServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IRepository<Group>> _groups = new();
    private readonly Mock<IRepository<GroupMember>> _members = new();
    private readonly Mock<IRepository<Expense>> _expenses = new();
    private readonly Mock<IRepository<ExpenseParticipant>> _participants = new();

    public GroupAndBalanceServiceTests()
    {
        _uow.Setup(u => u.Groups).Returns(_groups.Object);
        _uow.Setup(u => u.GroupMembers).Returns(_members.Object);
        _uow.Setup(u => u.Expenses).Returns(_expenses.Object);
        _uow.Setup(u => u.ExpenseParticipants).Returns(_participants.Object);
    }

    [Fact]
    public void GetGroupById_MissingGroup_ThrowsGroupNotFoundException()
    {
        // Arrange
        _groups.Setup(r => r.GetById(99)).Returns((Group?)null!);
        var service = new GroupService(_uow.Object);

        // Act & Assert
        Assert.Throws<GroupNotFoundException>(() => service.GetGroupById(99, currentUserId: 1));
    }

    [Fact]
    public void GetGroupById_UserNotInGroup_ThrowsUserNotInGroupException()
    {
        // Arrange: група 42 існує, але в ній лише користувач 1, а питає користувач 2 (IDOR)
        _groups.Setup(r => r.GetById(42)).Returns(new Group { Id = 42, Name = "Trip" });
        _members.Setup(r => r.GetAll()).Returns([new GroupMember { GroupId = 42, UserId = 1 }]);
        var service = new GroupService(_uow.Object);

        // Act & Assert
        Assert.Throws<UserNotInGroupException>(() => service.GetGroupById(42, currentUserId: 2));
    }

    [Fact]
    public void GetGroupBalances_InAnyGroup_SumToZero()
    {
        // Arrange: Піца 1000 грн — Аліса (1) заплатила 600, Боб (2) заплатив 400, їли четверо по 250
        _groups.Setup(r => r.GetById(1)).Returns(new Group { Id = 1, Name = "Pizza" });
        _members.Setup(r => r.GetAll()).Returns(
        [
            new GroupMember { GroupId = 1, UserId = 1 },
            new GroupMember { GroupId = 1, UserId = 2 },
            new GroupMember { GroupId = 1, UserId = 3 },
            new GroupMember { GroupId = 1, UserId = 4 }
        ]);
        _expenses.Setup(r => r.GetAll()).Returns([new Expense { Id = 10, GroupId = 1, Amount = 1000m }]);
        _participants.Setup(r => r.GetAll()).Returns(
        [
            new ExpenseParticipant { ExpenseId = 10, UserId = 1, PaidShare = 600m, OwedShare = 250m },
            new ExpenseParticipant { ExpenseId = 10, UserId = 2, PaidShare = 400m, OwedShare = 250m },
            new ExpenseParticipant { ExpenseId = 10, UserId = 3, PaidShare = 0m,   OwedShare = 250m },
            new ExpenseParticipant { ExpenseId = 10, UserId = 4, PaidShare = 0m,   OwedShare = 250m }
        ]);

        var service = new BalanceService(_uow.Object);

        var result = service.GetGroupBalances(groupId: 1, currentUserId: 1);

        Assert.Equal(0m, result.Balances.Sum(b => b.NetBalance));
        Assert.Equal(350m, result.Balances.First(b => b.UserId == 1).NetBalance);
        Assert.Equal(-250m, result.Balances.First(b => b.UserId == 3).NetBalance);
    }

    [Fact]
    public void SettleUp_ValidPayment_CreatesExpenseWithIsPaymentTrue()
    {
        // Arrange
        _groups.Setup(r => r.GetById(1)).Returns(new Group { Id = 1, Name = "Trip" });
        _members.Setup(r => r.GetAll()).Returns(
        [
            new GroupMember { GroupId = 1, UserId = 1 },
            new GroupMember { GroupId = 1, UserId = 2 }
        ]);
        var service = new GroupService(_uow.Object);

        // Act
        var settlement = service.SettleUp(1, new CreateSettlementDto(ToUserId: 2, Amount: 250m), currentUserId: 1);

        // Assert
        Assert.True(settlement.IsPayment);
        Assert.Equal(250m, settlement.Amount);
        _expenses.Verify(r => r.Add(It.Is<Expense>(e => e.IsPayment)), Times.Once);
        _uow.Verify(u => u.Save(), Times.Once);
    }
}