using System;

namespace Coimbra
{
    internal interface ISerializableDictionary : ISerializableCollection
    {
        bool IsNewEntryValid { get; }

        void ProcessAdd();

        void ProcessUndo();
    }
}
