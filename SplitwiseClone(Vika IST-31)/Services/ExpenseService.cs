using SplitwiseClone.Models;
using SplitwiseClone.Repositories;

namespace SplitwiseClone.Services;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExpenseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void AddExpense(Expense expense)
    {
        // Тут знаходиться бізнес-логіка: наприклад, перевірка чи сума більше 0
        if (expense.Amount <= 0)
        {
            throw new ArgumentException("Сума витрати має бути більшою за нуль.");
        }

        _unitOfWork.Expenses.Add(expense);
        _unitOfWork.Save();
    }

    public IEnumerable<Expense> GetAllExpenses()
    {
        return _unitOfWork.Expenses.GetAll();
    }
}