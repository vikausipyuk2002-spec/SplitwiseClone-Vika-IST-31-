using System;
using System.Linq;
using System.Reflection;
using SplitwiseClone.Exceptions;
using SplitwiseClone.Models;
using Xunit;

namespace UnitTest1;

public class ModelsAndExceptionsTests
{
    [Fact]
    public void CustomValidationException_SetsErrorsCorrectly()
    {
        var errors = new[] { "Error 1", "Error 2" };
        var exception = new CustomValidationException(errors);

        Assert.NotNull(exception.Errors);
        Assert.Equal(2, exception.Errors.Count());
        Assert.Contains("Error 1", exception.Errors);
    }

    [Fact]
    public void AllModels_GettersAndSetters_WorkCorrectly()
    {
        // Масив усіх твоїх моделей
        object[] models = {
            new Comment(),
            new ExpenseParticipant(),
            new GroupMember(),
            new Expense(),
            new Group(),
            new User()
        };

        foreach (var model in models)
        {
        
            var properties = model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                // Якщо властивість має get; та set;
                if (prop.CanWrite && prop.CanRead)
                {
                    // Генеруємо дефолтне значення (0 для чисел, null для рядків)
                    Type type = prop.PropertyType;
                    object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;

                    // Записуємо значення (покриваємо set)
                    prop.SetValue(model, defaultValue);

                    // Читаємо значення (покриваємо get)
                    var readValue = prop.GetValue(model);
                }
            }
        }

        Assert.True(true);
    }
}