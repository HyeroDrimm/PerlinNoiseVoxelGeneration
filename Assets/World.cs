using System;
using System.Collections.Generic;
public class World
{
    public Dictionary<ChunkId, Chunk> Chunks = new Dictionary<ChunkId, Chunk>();

    public UInt16 this[int x, int y, int z]
    {
        get
        {
            var chunk = Chunks[ChunkId.FromWorldPos(x, y, z)];
            return chunk[x & 0xF, y & 0xF, z & 0xF];
        }

        set
        {
            var chunk = Chunks[ChunkId.FromWorldPos(x, y, z)];
            chunk[x & 0xF, y & 0xF, z & 0xF] = value;
        }
    }
}