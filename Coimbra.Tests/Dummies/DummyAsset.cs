using UnityEngine;

namespace Coimbra.Tests
{
    public class DummyAsset : ScriptableObject, IDummyInterface
    {
        [SerializeField]
        private ManagedField<IDummyInterface> _managedField;

        [SerializeField]
        private int _integer;

        [SerializeField]
        private string _string;

        [SerializeField]
        private Vector3Int _vector;

        [SerializeField]
        private DummyStruct _struct;

        [SerializeField]
        private DummyAsset _asset;

        [SerializeField]
        private DummyBehaviour _behaviour;

        [SerializeField]
        private DummyClass _class;

        [SerializeField]
        private Texture _texture;

        [SerializeField]
        private int[] _integerArray;

        [SerializeField]
        private string[] _stringArray;

        [SerializeField]
        private Vector3Int[] _vectorArray;

        [SerializeField]
        private DummyStruct[] _structArray;

        [SerializeField]
        private DummyAsset[] _assetArray;

        [SerializeField]
        private DummyBehaviour[] _behaviourArray;

        [SerializeField]
        private DummyClass[] _classArray;

        [SerializeField]
        private Texture[] _textureArray;

        public ManagedField<IDummyInterface> ManagedField
        {
            get => _managedField;
            set => _managedField = value;
        }

        public DummyAsset Asset
        {
            get => _asset;
            set => _asset = value;
        }

        public DummyAsset[] AssetArray
        {
            get => _assetArray;
            set => _assetArray = value;
        }

        public DummyBehaviour Behaviour
        {
            get => _behaviour;
            set => _behaviour = value;
        }

        public DummyBehaviour[] BehaviourArray
        {
            get => _behaviourArray;
            set => _behaviourArray = value;
        }

        public DummyClass Class
        {
            get => _class;
            set => _class = value;
        }

        public DummyClass[] ClassArray
        {
            get => _classArray;
            set => _classArray = value;
        }

        public int Integer
        {
            get => _integer;
            set => _integer = value;
        }

        public int[] IntegerArray
        {
            get => _integerArray;
            set => _integerArray = value;
        }

        public string String
        {
            get => _string;
            set => _string = value;
        }

        public string[] StringArray
        {
            get => _stringArray;
            set => _stringArray = value;
        }

        public DummyStruct Struct
        {
            get => _struct;
            set => _struct = value;
        }

        public DummyStruct[] StructArray
        {
            get => _structArray;
            set => _structArray = value;
        }

        public Texture Texture
        {
            get => _texture;
            set => _texture = value;
        }

        public Texture[] TextureArray
        {
            get => _textureArray;
            set => _textureArray = value;
        }

        public Vector3Int Vector
        {
            get => _vector;
            set => _vector = value;
        }

        public Vector3Int[] VectorArray
        {
            get => _vectorArray;
            set => _vectorArray = value;
        }
    }
}
