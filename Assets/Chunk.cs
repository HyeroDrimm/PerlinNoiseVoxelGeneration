using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Chunk : MonoBehaviour
{
    private UInt16[] _voxels = new ushort[16 * 16 * 16];
    private MeshFilter _meshFilter;

    public World _world;

    public Material _material;

    public UInt16 this[int x, int y, int z]
    {
        get { return _voxels[x * 16 * 16 + y * 16 + z]; }
        set { _voxels[x * 16 * 16 + y * 16 + z] = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
        GetComponent<MeshRenderer>().material = _material;
    }

    private void Update()
    {

            RenderToMesh();
    }
    public void RenderToMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = 0; z < 16; z++)
                {
                    // If it is air we ignore this block
                    var voxelType = this[x, y, z];
                    if (voxelType == 0)
                        continue;

                    var pos = new Vector3(x, y, z);
                    // Remember current position in vertices list so we can add triangles relative to that
                    var verticesPos = vertices.Count;
                    var trianglesPos = triangles.Count;

                    for (int i = 0; i < 6; i++)
                    {
                        Vector3 checkPos = _directions[i] + new Vector3(x, y, z);
                        
                        if (checkPos.x == -1 || checkPos.x == 16 || checkPos.y == -1 || checkPos.y == 16 || checkPos.z == -1 || checkPos.z == 16)
                        {
                            Vector3 checkedChunk = transform.position / 16 + _directions[i];
                            
                            if (_world.Chunks.ContainsKey(new ChunkId((int)checkedChunk.x, (int)checkedChunk.y, (int)checkedChunk.z)))
                            {
                                Vector3 checkedVoxel = new Vector3(x, y, z) + transform.position + _directions[i];
                                if (_world[(int)checkedVoxel.x, (int)checkedVoxel.y, (int)checkedVoxel.z] == 0)
                                {
                                    foreach (var tri in GetDirectionTriangles(i))
                                        triangles.Add(verticesPos + tri);
                                }
                            }
                            else
                            {
                                foreach (var tri in GetDirectionTriangles(i))
                                    triangles.Add(verticesPos + tri);
                            }
                            
                        }
                        else if (this[(int)checkPos.x, (int)checkPos.y, (int)checkPos.z] == 0)
                        {
                            foreach (var tri in GetDirectionTriangles(i))
                                triangles.Add(verticesPos + tri);
                        }
                    }

                    // If no verticies were added no triangles are added
                    if (trianglesPos == triangles.Count)
                    {
                        continue;
                    }
                    foreach (var vert in _cubeVertices)
                        vertices.Add(pos + vert); // Voxel postion + cubes vertex
                }
            }
        }

        // Apply new mesh to MeshFilter
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.RecalculateNormals();
        _meshFilter.mesh = mesh;
    }

    private static Vector3[] _cubeVertices = new[] {
         new Vector3 (0, 0, 0),
         new Vector3 (1, 0, 0),
         new Vector3 (1, 1, 0),
         new Vector3 (0, 1, 0),
         new Vector3 (0, 1, 1),
         new Vector3 (1, 1, 1),
         new Vector3 (1, 0, 1),
         new Vector3 (0, 0, 1),
    };

    private static int[] _cubeTriangles = new[] {
        // Front
         0, 2, 1,
         0, 3, 2,
        // Top
         2, 3, 4,
         2, 4, 5,
        // Right
         1, 2, 5,
         1, 5, 6,
        // Left
         0, 7, 4,
         0, 4, 3,
        // Back
         5, 4, 7,
         5, 7, 6,
        // Bottom
         0, 6, 7,
         0, 1, 6
    };

    private static int[] GetDirectionTriangles(int num)
    {
        int[] triangles = new int[6];
        for (int i = 0; i < 6; i++)
        {
            triangles[i] = _cubeTriangles[i + num * 6];
        }
        return triangles;
    }

    private static Vector3[] _directions = new Vector3[] {
        Vector3.back,
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.down,
    };
}