using UnityEngine;

namespace Coimbra
{
    internal interface ISerializableDictionary : ISerializationCallbackReceiver
    {
        bool CanAdd { get; }

        void Add();
    }
}
