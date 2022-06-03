namespace PSQuickAssets.Assets;

/// <summary>
/// Result of the operation.
/// </summary>
/// <typeparam name="T">Type of an output.</typeparam>
/// <param name="IsSuccessful">Indicates whether operation was successful or not.</param>
/// <param name="Output">"Return Value" of an operation.</param>
/// <param name="Exception">If operation wasn't successful because of an exception - it will be stored here.</param>
public record Result<T>(bool IsSuccessful, T Output, Exception? Exception = null);
/// <summary>
/// Result of the operation.
/// </summary>
/// <param name="IsSuccessful">Indicates whether operation was successful or not.</param>
/// <param name="Exception">If operation wasn't successful because of an exception - it will be stored here.</param>
public record Result(bool IsSuccessful, Exception? Exception = null);