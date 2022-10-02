using System;
using UnityEngine;
public struct ChunkId : IEquatable<ChunkId>
{
    public readonly int x;
    public readonly int y;
    public readonly int z;

    public ChunkId(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public static explicit operator Vector3Int(ChunkId obj)
    {
        return new Vector3Int(obj.x, obj.y, obj.z);
    }
    
    public static ChunkId FromWorldPos(int x, int y, int z)
    {
        return new ChunkId(x >> 4, y >> 4, z >> 4);
    }

    #region Equality members

    public bool Equals(ChunkId other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is ChunkId other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = x;
            hashCode = (hashCode * 397) ^ y;
            hashCode = (hashCode * 397) ^ z;
            return hashCode;
        }
    }

    public static bool operator ==(ChunkId left, ChunkId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ChunkId left, ChunkId right)
    {
        return !left.Equals(right);
    }

    #endregion
}