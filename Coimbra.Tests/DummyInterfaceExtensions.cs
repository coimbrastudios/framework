using UnityEngine;

namespace Coimbra.Tests
{
    internal static class DummyInterfaceExtensions
    {
        public static void Initialize<T>(this T target, DummyAsset[] assets, DummyBehaviour[] behaviours, int[] integers, string[] strings, Texture[] textures, Vector3Int[] vectors)
            where T : IDummyInterface
        {
            target.Asset = assets?[Random.Range(0, assets.Length)];
            target.Behaviour = behaviours?[Random.Range(0, behaviours.Length)];
            target.Integer = integers?[Random.Range(0, integers.Length)] ?? default;
            target.String = strings?[Random.Range(0, strings.Length)];
            target.Texture = textures?[Random.Range(0, textures.Length)];
            target.Vector = vectors?[Random.Range(0, vectors.Length)] ?? default;
            target.AssetArray = assets;
            target.BehaviourArray = behaviours;
            target.IntegerArray = integers;
            target.StringArray = strings;
            target.TextureArray = textures;
            target.VectorArray = vectors;
        }
    }
}
