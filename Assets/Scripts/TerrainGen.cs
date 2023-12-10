using sc.terrain.vegetationspawner;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;


// TODO cite all of this
// https://www.youtube.com/watch?v=vFvwyu_ZKfU&ab_channel=Brackeys
// https://www.youtube.com/watch?v=WP-Bm65Q-1Y&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=2&ab_channel=SebastianLague
// https://discussions.unity.com/t/how-to-automatically-apply-different-textures-on-terrain-based-on-height/2013/2


public class TerrainGen : MonoBehaviour
{
    // Spawner for automatically placing trees and grass
    public VegetationSpawner spawner;

    // Physical dimensions of terrain
    public int width = 500;
    public int height = 500;
    public int depth = 10;

    // Height generation parameters
    public float scale = 20f;
    public float offsetX;
    public float offsetY;

    // Extra detail parameters
    public int octaves = 3;
    [Range(0f, 1f)]
    public float persistence = 0.5f;    // how much the amplitude increases per octave
    public float lacunarity = 2f;       // how much the frequency changes per octave
    [Range(0f, 1f)]
    public float heightThreshold = 0.6f;  // Height at which texture will be grass instead of dirt


    public void Generate() {
        // Randomize offset so we get different terrain each time
        offsetX = Random.Range(0f, 9999f);
        offsetY = Random.Range(0f, 9999f);

        // Get reference to terrain, generate heights and color
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
        terrain.terrainData = ChangeTerrainTexture(terrain.terrainData);

        // Respawn trees and grass to match the generated terrain
        spawner.Respawn();
    }

    // Color the terrain depending based on its height
    TerrainData ChangeTerrainTexture(TerrainData td)
    {
        float[,] heights = td.GetHeights(0, 0, td.heightmapResolution, td.heightmapResolution);
        float[,,] splatMap = new float[td.alphamapWidth, td.alphamapHeight, td.alphamapLayers];

        for (int x = 0; x < td.alphamapWidth; x++) {
            for (int y = 0; y < td.alphamapHeight; y++) {
                float terrainHeight = heights[x * td.heightmapResolution / td.alphamapWidth, y * td.heightmapResolution / td.alphamapHeight];

                // Check if the terrain height is below the threshold
                if (terrainHeight < heightThreshold) {
                    splatMap[x, y, 0] = 0; // Set low heights to texture 1 (dirt)
                    splatMap[x, y, 1] = 1;
                }
                else if (terrainHeight < heightThreshold + 0.1) { // mix textures for medium heights
                    splatMap[x, y, 0] = 0.5f;
                    splatMap[x, y, 1] = 0.5f;
                }
                else {
                    splatMap[x, y, 0] = 1; // Set large heights to texture 2 (grass)
                    splatMap[x, y, 1] = 0;
                }
            }
        }

        td.SetAlphamaps(0, 0, splatMap);
        return td;
    }

    // Generate the terrain
    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    // Generate a height for each point on the terrain
    float[,] GenerateHeights() {

        // Use layered Perlin Noise to generate terrain
        float[,] heights = new float[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // Convert coords to floats + add random offset
                float xcoord = (float)x / width * scale + offsetX;
                float ycoord = (float)y / height * scale + offsetY;

                // Layer multiple Perlin noises on top of each other at different frequencies
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    float perlin = Mathf.PerlinNoise(xcoord * frequency, ycoord * frequency);
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[x, y] = noiseHeight;
            }
        }

        heights = PostProcess(heights);

        return heights;
    }

    // Post-process terrain so there is a flat area at the center for a campsite
    float[,] PostProcess(float[,] heights) {
        float k = 0.00003f; // found via trial and error

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float d = Mathf.Sqrt((width/2 - x) * (width/2 - x) + (height/2 - y) * (height/2 - y));


                if (d < 10) { // Force campsite height to 0.5
                    heights[x, y] = 0.5f;
                }
                else if (d >= 10 && d <= 20) { // smooth transition from campsite to surrounding terrain
                    float scale = 1 - Mathf.Exp(-k * Mathf.Pow(d, 4));
                    scale = Mathf.Clamp(scale, 0.5f, 1f);
                    heights[x, y] = Mathf.Clamp(heights[x,y] * scale, 0.5f, 1);
                }

            }
        }
        return heights;
    }

}
    


