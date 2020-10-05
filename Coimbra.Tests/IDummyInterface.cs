using UnityEngine;

namespace Coimbra
{
    public interface IDummyInterface
    {
        public DummyAsset Asset { get; set; }

        public DummyAsset[] AssetArray { get; set; }

        public DummyBehaviour Behaviour { get; set; }

        public DummyBehaviour[] BehaviourArray { get; set; }

        public int Integer { get; set; }

        public int[] IntegerArray { get; set; }

        public string String { get; set; }

        public string[] StringArray { get; set; }

        public Texture Texture { get; set; }

        public Texture[] TextureArray { get; set; }

        public Vector3Int Vector { get; set; }

        public Vector3Int[] VectorArray { get; set; }
    }
}
