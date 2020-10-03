using UnityEngine;

namespace Coimbra
{
    public sealed class DummyAssetWithDummyInterface : ScriptableObject, IDummyInterface
    {
#if !UNITY_2020_1_OR_NEWER
        [SerializeField] private DummyInterfaceField _interfaceField = new DummyInterfaceField();
#else
        [SerializeField] private InterfaceField<IDummyInterface> _interfaceField;
#endif

        public int Number { get; set; }

        public InterfaceField<IDummyInterface> InterfaceField
        {
            get => _interfaceField;
            set => _interfaceField = value;
        }
    }
}
