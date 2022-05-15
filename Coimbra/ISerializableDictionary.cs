using System;

namespace Coimbra
{
    internal interface ISerializableDictionary
    {
        bool IsPairValid { get; }

        Type KeyType { get; }

        Type ValueType { get; }

        void ProcessAdd();

        void ProcessUndo();
    }
}
