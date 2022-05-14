namespace Coimbra
{
    /// <summary>
    /// Implement this interface to make the type compatible with <see cref="SortableComparer"/>.
    /// </summary>
    public interface ISortable
    {
        int Order { get; }
    }
}
