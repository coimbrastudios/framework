using System;

namespace Coimbra
{
    /// <summary>
    /// Similar to <see cref="EventHandler{TEventArgs}"/> but using the ref keyword.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public delegate void EventRefHandler<T>(object sender, ref T e);
}
