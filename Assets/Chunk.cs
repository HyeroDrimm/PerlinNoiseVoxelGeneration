using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
	public int chunkSize = 16;
	public ChunkId ChunkId;
	private UInt16[] voxels;
	private MeshFilter meshFilter;

	public World World;

	public Material material;
	private MeshRenderer meshRenderer;

	public UInt16 this[int x, int y, int z] { get => voxels[x * chunkSize * chunkSize + y * chunkSize + z]; set => voxels[x * chunkSize * chunkSize + y * chunkSize + z] = value; }

	// Start is called before the first frame update
	private void Awake()
	{
		voxels = new ushort[chunkSize * chunkSize * chunkSize];
		meshFilter = GetComponent<MeshFilter>();
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Start()
	{
		meshRenderer.material = material;
	}

	private void Update()
	{
		RenderToMesh();
	}

	public void RenderToMesh()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();

		for (int x = 0; x < chunkSize; x++)
		for (int y = 0; y < chunkSize; y++)
		for (int z = 0; z < chunkSize; z++)
		{
			// If it is air we ignore this block
			var voxelType = this[x, y, z];
			if (voxelType == 0)
				continue;

			var pos = new Vector3Int(x, y, z);
			// Remember current position in vertices list so we can add triangles relative to that
			var verticesPos = vertices.Count;
			var trianglesPos = triangles.Count;

			for (int i = 0; i < 6; i++)
			{
				Vector3Int checkPos = Directions[i] + pos;

				if (checkPos.x == -1 || checkPos.x == chunkSize || checkPos.y == -1 || checkPos.y == chunkSize || checkPos.z == -1 || checkPos.z == chunkSize)
				{
					Vector3Int checkedChunk = (Vector3Int)ChunkId + Directions[i];

					if (World.Chunks.ContainsKey(new ChunkId(checkedChunk.x, checkedChunk.y, checkedChunk.z)))
					{
						Vector3Int checkedVoxel = pos + (Vector3Int)ChunkId * chunkSize + Directions[i];
						if (World[checkedVoxel.x, checkedVoxel.y, checkedVoxel.z] == 0)
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
				else if (this[checkPos.x, checkPos.y, checkPos.z] == 0)
				{
					foreach (var tri in GetDirectionTriangles(i))
						triangles.Add(verticesPos + tri);
				}
			}

			// If no vertices were added no triangles are added
			if (trianglesPos == triangles.Count)
			{
				continue;
			}

			foreach (var vert in CubeVertices)
				vertices.Add(pos + vert); // Voxel position + cubes vertex
		}


		// Apply new mesh to MeshFilter
		var mesh = new Mesh();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles.ToArray(), 0);
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
	}

	private static readonly Vector3[] CubeVertices =
	{
		new Vector3(0, 0, 0),
		new Vector3(1, 0, 0),
		new Vector3(1, 1, 0),
		new Vector3(0, 1, 0),
		new Vector3(0, 1, 1),
		new Vector3(1, 1, 1),
		new Vector3(1, 0, 1),
		new Vector3(0, 0, 1),
	};

	private static readonly int[] CubeTriangles =
	{
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
			triangles[i] = CubeTriangles[i + num * 6];
		}

		return triangles;
	}

	private static readonly Vector3Int[] Directions =
	{
		Vector3Int.back,
		Vector3Int.up,
		Vector3Int.right,
		Vector3Int.left,
		Vector3Int.forward,
		Vector3Int.down,
	};

}