using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshFilter))]

public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 25;
    public const int ChunkHeight = 128;
    public const float BlockScale = .25f;
    
    public ChunkData ChunkData;
    public GameWorld ParrentWorld;

    private Mesh chunkMesh;
    
    private List<Vector3> verticies = new List<Vector3>();
    private List<int> triangles = new List<int>();
    
    void Start()
    {
        chunkMesh = new Mesh();

        RegenerateMesh();

        GetComponent<MeshFilter>().sharedMesh = chunkMesh;

    }

    private void RegenerateMesh()
    {
        verticies.Clear();
        triangles.Clear();
        
        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateCube(x, y, z);
                }
            }
        }

        chunkMesh.triangles = new int[] {};
        
        chunkMesh.vertices = verticies.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();
        
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Grass;
        RegenerateMesh();
    }
    
    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }
    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkWidth) return BlockType.Air;
            
            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition;
            if(blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if(blockPosition.x >= ChunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }
            
            if(blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if(blockPosition.z >= ChunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if (ParrentWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }

    public void GenerateCube(int x, int y, int z)
    {
        var blockPostion = new Vector3Int(x, y, z);

        if (GetBlockAtPosition(blockPostion) == 0) return;

        if (GetBlockAtPosition(blockPostion + Vector3Int.right) == 0) GenerateRightSide(blockPostion);
        if (GetBlockAtPosition(blockPostion + Vector3Int.left) == 0) GenerateLeftSide(blockPostion);
        if (GetBlockAtPosition(blockPostion + Vector3Int.forward) == 0) GenerateFrontSide(blockPostion);
        if (GetBlockAtPosition(blockPostion + Vector3Int.back) == 0) GenerateBackSide(blockPostion);
        if (GetBlockAtPosition(blockPostion + Vector3Int.up) == 0) GenerateToptSide(blockPostion);
        if (GetBlockAtPosition(blockPostion + Vector3Int.down) == 0) GenerateBottomSide(blockPostion);

         //GenerateRightSide(blockPostion);
         //GenerateLeftSide(blockPostion);
         //GenerateFrontSide(blockPostion);
         //GenerateBackSide(blockPostion);
         //GenerateToptSide(blockPostion);
         //GenerateBottomSide(blockPostion);
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale); 
        
        AddLastVerticlesSquare();
    }
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticlesSquare();
    }
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale); 
    
        AddLastVerticlesSquare();
    }
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVerticlesSquare();
    }
    private void GenerateToptSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticlesSquare();
    }
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        verticies.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        verticies.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);

        AddLastVerticlesSquare();
    }
    private void AddLastVerticlesSquare()
    {
        triangles.Add(verticies.Count - 4);
        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 2);

        triangles.Add(verticies.Count - 3);
        triangles.Add(verticies.Count - 1);
        triangles.Add(verticies.Count - 2);
    }
}
