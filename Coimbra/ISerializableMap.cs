using System;

namespace Coimbra
{
    internal interface ISerializableMap
    {
        bool IsPairValid { get; }

        Type KeyType { get; }

        Type ValueType { get; }

        void ProcessAdd();

        void ProcessUndo();
    }
}
