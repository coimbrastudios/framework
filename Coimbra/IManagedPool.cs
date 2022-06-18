namespace Coimbra
{
    internal interface IManagedPool
    {
        int AvailableCount { get; }

        int MaxCapacity { get; }

        int PreloadCount { get; }
    }
}
