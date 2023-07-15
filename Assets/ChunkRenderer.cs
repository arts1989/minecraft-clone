using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshFilter))]

public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 32;
    public const int ChunkHeight = 128;
    public const float BlockScale = .125f;
    
    public ChunkData ChunkData;
    public GameWorld ParrentWorld;

    //public BlockInfo[] Blocks;
    public BlockDatabase Blocks;

    private Mesh chunkMesh;
    
    private List<Vector3> verticies = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
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
        uvs.Clear();
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
        chunkMesh.uv = uvs.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();
        
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Stone;
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
        Vector3Int blockPostion = new Vector3Int(x, y, z);

        BlockType blockType = GetBlockAtPosition(blockPostion);
        if (blockType == BlockType.Air) return;

        if (GetBlockAtPosition(blockPostion + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPostion);
            AddUvs(blockType);
        }

        if (GetBlockAtPosition(blockPostion + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPostion);
            AddUvs(blockType);
        }

        if (GetBlockAtPosition(blockPostion + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPostion);
            AddUvs(blockType);
        }

        if (GetBlockAtPosition(blockPostion + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPostion);
            AddUvs(blockType);
        }

        if (GetBlockAtPosition(blockPostion + Vector3Int.up) == 0)
        {
            GenerateToptSide(blockPostion);
            AddUvs(blockType, Vector3Int.up);
        }

        if (GetBlockAtPosition(blockPostion + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPostion);
            AddUvs(blockType, Vector3Int.down);
        }

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

    private void AddUvs(BlockType blockType, Vector3Int normal = new Vector3Int())
    {
        Vector2 uv;

        //BlockInfo info = Blocks.GetInfo(blockType);
        //BlockInfo info = Blocks.Blocks.FirstOrDefault(b => b.Type == blockType);
        /*if (info != null)
        {
            uv = info.GetPixelOffset(normal) / 512;
            //uv = new Vector2(info.PixelsOffset.x / 512, info.PixelsOffset.y / 512);
        }
        else
        {
            uv = new Vector2(288f / 512, 416f / 512);
        }*/

        if (blockType == BlockType.Grass)
        {
            if (normal == Vector3Int.up)
            {
                uv = new Vector2(64f / 512, 192f / 512);
            }
            else if(normal == Vector3Int.down)
            {
                uv = new Vector2(64f / 512, 480f / 512);
            }
            else
            {
                uv = new Vector2(96f / 512, 480f / 512);
            }
        }
        else
        {
            uv = new Vector2(288f / 512, 416f / 512);
        }
        
        for (int i = 0; i < 4; i++)
        {
            uvs.Add(uv);
        }
    }
}
