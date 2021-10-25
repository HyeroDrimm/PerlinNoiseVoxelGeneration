using UnityEngine;

public class VoxelEngine : MonoBehaviour
{
    private World _world = new World();
    private System.Random _random = new System.Random();


    public int numberOfChunks = 1;
    public float treshold = 0.5f;
    public float scale = 1;
    public Vector3 displacement;

    public Material mat;


    // Start is called before the first frame update
    void Start()
    {
        for (var x = 0; x < numberOfChunks; x++)
        {
            for (var y = 0; y < numberOfChunks; y++)
            {
                for (var z = 0; z < numberOfChunks; z++)
                {
                    // Create GameObject that will hold a Chunk
                    GameObject chunkGameObject = new GameObject("Chunk " + x + " " + y + " " + z);
                    chunkGameObject.transform.parent = transform.parent;
                    chunkGameObject.transform.position += new Vector3(x * 16, y * 16, z * 16);
                    // Add Chunk to GameObject
                    Chunk chunk = chunkGameObject.AddComponent<Chunk>();
                    chunk._world = _world;
                    chunk._material = mat;
                    // Add chunk to world at position 0, 0, 0
                    _world.Chunks.Add(new ChunkId(x, y, z), chunk);
                }
            }
        }

        //print(ChunkId.FromWorldPos(32, -64, 16));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RefreshChunks();
        }   
    }

    public void RefreshChunks()
    {
        for (int x = 0; x < 16 * numberOfChunks; x++)
        {
            for (int y = 0; y < 16 * numberOfChunks; y++)
            {
                for (int z = 0; z < 16 * numberOfChunks; z++)
                {
                    if (Perlin3D(x, y, z, scale, displacement) >= treshold)
                    {
                        _world[x, y, z] = 1;
                    }
                    else
                        _world[x, y, z] = 0;
                }
            }
        }
        foreach (var chunk in _world.Chunks)
        {
            chunk.Value.RenderToMesh();
        }

    }

    public static float Perlin3D(float x, float y, float z, float scale, Vector3 displacement)
    {
        x = (x + displacement.x) * scale * 0.1f;
        y = (y + displacement.y) * scale * 0.1f;
        z = (z + displacement.z) * scale * 0.1f;

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