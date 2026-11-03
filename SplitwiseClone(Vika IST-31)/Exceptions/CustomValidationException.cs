using System;
using System.Collections.Generic;

namespace SplitwiseClone.Exceptions;

public class CustomValidationException : Exception
{
    public IEnumerable<string> Errors { get; }

    public CustomValidationException(IEnumerable<string> errors)
        : base("Помилка валідації даних.")
    {
        Errors = errors;
    }
}