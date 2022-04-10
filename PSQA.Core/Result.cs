using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSQA.Core;

/// <summary>
/// Represents a result of an operation.
/// </summary>
public record Result
{
    /// <summary>
    /// Indicates whether the operation was a success.
    /// </summary>
    public bool Success { get; }
    /// <summary>
    /// Gets the message of what went wrong when operation failed.
    /// </summary>
    public string Error { get; }
    /// <summary>
    /// Indicates whether the operation was a failure.
    /// </summary>
    public bool Failure { get => !Success; }

    /// <summary>
    /// Creates instance of class and sets specified properties.
    /// </summary>
    /// <param name="success">Whether operation succeded or not.</param>
    /// <param name="errorMessage">Message of the fail.</param>
    /// <exception cref="InvalidOperationException">If success is <see langword="true"/> and message is not empty.</exception>
    /// <exception cref="InvalidOperationException">If success is <see langword="false"/> and message is empty.</exception>
    protected Result(bool success, string errorMessage)
    {
        if (success && errorMessage != string.Empty)
            throw new InvalidOperationException("Successful result cannot have an error message.");

        if (!success && string.IsNullOrWhiteSpace(errorMessage))
            throw new InvalidOperationException("Failed result should have an error message.");

        Success = success;
        Error = errorMessage;
    }

    /// <summary>
    /// Operation was successful.
    /// </summary>
    public static Result Ok() => new Result(true, string.Empty);
    /// <summary>
    /// Operation failed.
    /// </summary>
    public static Result Fail(string errorMessage) => new Result(false, errorMessage);

    public static Result<T> Ok<T>(T value) => new Result<T>(value, true, string.Empty);
    public static Result<T> Fail<T>(string errorMessage) => new Result<T>(default!, false, errorMessage);
}

/// <summary>
/// Represents a result of an operation with a value.
/// </summary>
/// <typeparam name="T">Type of the value.</typeparam>
public record Result<T> : Result
{
    /// <summary>
    /// Gets the value if result was a success.<br></br>
    /// <see langword="null"/> if result is failure.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Creates instance of class and sets specified properties.
    /// </summary>
    /// <param name="value">Value of the result.</param>
    /// <param name="success">Whether operation succeded or not.</param>
    /// <param name="errorMessage">Message of the fail.</param>
    protected internal Result(T value, bool success, string errorMessage) : base(success, errorMessage)
    {
        Value = value;
    }
}