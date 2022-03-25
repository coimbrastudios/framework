using UnityEngine;

namespace Coimbra.Tests
{
    public interface IDummyInterface
    {
        DummyAsset Asset { get; set; }

        DummyAsset[] AssetArray { get; set; }

        DummyBehaviour Behaviour { get; set; }

        DummyBehaviour[] BehaviourArray { get; set; }

        int Integer { get; set; }

        int[] IntegerArray { get; set; }

        string String { get; set; }

        string[] StringArray { get; set; }

        Texture Texture { get; set; }

        Texture[] TextureArray { get; set; }

        Vector3Int Vector { get; set; }

        Vector3Int[] VectorArray { get; set; }
    }
}
