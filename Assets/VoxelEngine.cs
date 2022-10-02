using UnityEngine;

public class VoxelEngine : MonoBehaviour
{
	[SerializeField] private int chunkSize = 16;
	[SerializeField] private int numberOfChunks = 1;
	[SerializeField] private float threshold = 0.5f;
	[SerializeField] private float scale = 1;
	[SerializeField] private Vector3 displacement;
	[SerializeField] private Material mat;
	
	private readonly World world = new World();

	private void Start()
	{
		for (int x = 0; x < numberOfChunks; ++x)
		for (int y = 0; y < numberOfChunks; ++y)
		for (int z = 0; z < numberOfChunks; ++z)
		{
			// Create GameObject that will hold a Chunk
			GameObject chunkGameObject = new GameObject($"Chunk {x} {y} {z}");
			chunkGameObject.transform.parent = transform.parent;
			chunkGameObject.transform.Translate(x * chunkSize, y * chunkSize, z * chunkSize);
			// Add Chunk to GameObject
			var chunkID = new ChunkId(x, y, z);

			Chunk chunk = chunkGameObject.AddComponent<Chunk>();
			chunk.World = world;
			chunk.material = mat;
			chunk.chunkSize = chunkSize;
			chunk.ChunkId = chunkID;

			world.Chunks.Add(chunkID, chunk);
		}
		
		RefreshChunks();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			RefreshChunks();
	}

	private void RefreshChunks()
	{
		for (int x = 0; x < chunkSize * numberOfChunks; x++)
		for (int y = 0; y < chunkSize * numberOfChunks; y++)
		for (int z = 0; z < chunkSize * numberOfChunks; z++)
			world[x, y, z] = (ushort)(Perlin3D(x, y, z, scale, displacement) >= threshold ? 1 : 0);

		foreach (var chunk in world.Chunks)
		{
			chunk.Value.RenderToMesh();
		}
	}

	private static float Perlin3D(float x, float y, float z, float scale, Vector3 displacement)
	{
		x = (x + displacement.x) * scale * 0.01f;
		y = (y + displacement.y) * scale * 0.01f;
		z = (z + displacement.z) * scale * 0.01f;

		float ab = Mathf.PerlinNoise(x, y);
		float bc = Mathf.PerlinNoise(y, z);
		float ac = Mathf.PerlinNoise(x, z);

		float ba = Mathf.PerlinNoise(y, x);
		float cb = Mathf.PerlinNoise(z, y);
		float ca = Mathf.PerlinNoise(z, x);

		float abc = ab + bc + ac + ba + cb + ca;

		return abc / 6f;
	}
}