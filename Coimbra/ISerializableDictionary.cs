using System;

namespace Coimbra
{
    internal interface ISerializableDictionary : ISerializableCollection
    {
        bool IsNewEntryValid { get; }

        Type KeyType { get; }

        Type ValueType { get; }

        void ProcessAdd();

        void ProcessUndo();
    }
}
