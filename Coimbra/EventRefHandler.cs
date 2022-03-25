using System;

namespace Coimbra
{
    /// <summary>
    /// Similar to <see cref="EventHandler{TEventArgs}"/> but using the ref keyword.
    /// </summary>
    public delegate void EventRefHandler<T>(object sender, ref T e);
}
