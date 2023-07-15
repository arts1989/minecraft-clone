using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/block db")]

public class BlockDatabase : ScriptableObject
{
    public BlockInfo[] Blocks;

    private Dictionary<BlockType, BlockInfo> blocksCached = new Dictionary<BlockType, BlockInfo>();

    private void Init()
    {
        blocksCached.Clear();

        foreach (var blockInfo in Blocks)
        {
           blocksCached.Add(blockInfo.Type, blockInfo);
        }
    }

    public BlockInfo GetInfo(BlockType type)
    {
        if (blocksCached.Count == 0) Init();
        
        if (blocksCached.TryGetValue(type, out var blockInfo))
        {
            return blockInfo;
        }

        return null;
    }
}
