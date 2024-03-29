﻿using UnityEngine;

[CreateAssetMenu(menuName = "Blocks/Normal block")]
public class BlockInfo : ScriptableObject
{
    public BlockType Type;
    public Vector2 PixelsOffset;

    public AudioClip StepSound;
    public float TimeToBreak = 0.3f;

    public virtual Vector2 GetPixelOffset(Vector3Int normal)
    {
        return PixelsOffset;
    }
        
}
 