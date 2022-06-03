namespace PSQA.Core;

/// <summary>
/// Defines a photoshop action.
/// </summary>
public record PhotoshopAction
{
    /// <summary>
    /// Gets the action name.
    /// </summary>
    public string Action { get; init; }
    /// <summary>
    /// Gets set name, to what this action belongs to.
    /// </summary>
    public string Set { get; init; }

    /// <summary>
    /// Action with its name and set empty.
    /// </summary>
    public static PhotoshopAction Empty { get; } = new PhotoshopAction(string.Empty, string.Empty);

    /// <summary>
    /// Creates instance of class.
    /// </summary>
    public PhotoshopAction(string action, string set)
    {
        Action = action;
        Set = set;
    }

    /// <summary>
    /// Creates instance of class.
    /// </summary>
    public PhotoshopAction() : this(string.Empty, string.Empty) { }

    public override string ToString()
    {
        return $"{Action} of {Set}";
    }
}