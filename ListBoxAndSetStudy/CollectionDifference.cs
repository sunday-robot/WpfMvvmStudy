namespace ListBoxAndSetStudy;

public abstract record CollectionDifference()
{
    public sealed record Add(object Value) : CollectionDifference();
    public sealed record Update(object OldValue, object NewValue) : CollectionDifference();
    public sealed record Delete(object Value) : CollectionDifference();
}
