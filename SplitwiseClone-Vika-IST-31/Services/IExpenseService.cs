using SplitwiseClone.Models;

namespace SplitwiseClone.Services;

public interface IExpenseService
{
    void AddExpense(Expense expense);
    IEnumerable<Expense> GetAllExpenses();
}