namespace PSQuickAssets.Models;

public record PhotoshopAction
{
    public string Action { get; init; }
    public string Set { get; init; }

    public static PhotoshopAction Empty { get; } = new PhotoshopAction(string.Empty, string.Empty);

    public PhotoshopAction(string action, string set)
    {
        Action = action;
        Set = set;
    }

    public PhotoshopAction() : this(string.Empty, string.Empty) { }

    public override string ToString()
    {
        return $"{Action} of {Set}";
    }
}