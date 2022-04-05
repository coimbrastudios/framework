using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Coimbra
{
    /// <summary>
    /// Strongly-typed version of the <see cref="UnityEngine.GameObject"/>'s <see cref="UnityEngine.Object.GetInstanceID"/>.
    /// </summary>
    [Preserve]
    public readonly struct GameObjectID : IEquatable<GameObjectID>
    {
        public readonly int Id;

        public GameObjectID(int id)
        {
            Id = id;
        }

        public static implicit operator GameObjectID(GameObject gameObject)
        {
            return new GameObjectID(gameObject.GetInstanceID());
        }

        public static bool operator ==(GameObjectID left, GameObjectID right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GameObjectID left, GameObjectID right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is GameObjectID other && Equals(other);
        }

        public bool Equals(GameObjectID other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
