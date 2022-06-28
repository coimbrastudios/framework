using System;
using UnityEngine;

namespace Coimbra.Tests
{
    [Serializable]
    public class DummyClass : IDummyInterface
    {
        [SerializeField]
        private int _integer;

        [SerializeField]
        private string _string;

        [SerializeField]
        private Vector3Int _vector;

        [SerializeField]
        private DummyAsset _asset;

        [SerializeField]
        private DummyBehaviour _behaviour;

        [SerializeField]
        private Texture _texture;

        [SerializeField]
        private int[] _integerArray;

        [SerializeField]
        private string[] _stringArray;

        [SerializeField]
        private Vector3Int[] _vectorArray;

        [SerializeField]
        private DummyAsset[] _assetArray;

        [SerializeField]
        private DummyBehaviour[] _behaviourArray;

        [SerializeField]
        private Texture[] _textureArray;

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
