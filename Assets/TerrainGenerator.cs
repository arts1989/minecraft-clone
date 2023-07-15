using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain generator")]
public class TerrainGenerator : ScriptableObject
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;
    public NoiseOctaveSettings DomainWarp;
    
    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[] octaveNoises;
    private FastNoiseLite wrapNoise;

    public void Init()
    {
        octaveNoises = new FastNoiseLite[Octaves.Length];
        for (int i = 0; i < Octaves.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
            octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }

        wrapNoise = new FastNoiseLite();
        wrapNoise.SetNoiseType(DomainWarp.NoiseType);
        wrapNoise.SetFrequency(DomainWarp.Frequency);
        wrapNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }

    public BlockType[,,] GenerateTerrain( float xOffset, float zOffset)
    {
        var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = GetHeight(x * ChunkRenderer.BlockScale + xOffset, z * ChunkRenderer.BlockScale + zOffset);
                
                for (int y = 0; y < height / ChunkRenderer.BlockScale; y++)
                {
                    result[x, y, z] = BlockType.Grass;
                }
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        wrapNoise.DomainWarp(ref x, ref y);
        
        float result = BaseHeight;
        
        for (int i = 0; i < Octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude / 2;
        }

        return result;
    }
}
