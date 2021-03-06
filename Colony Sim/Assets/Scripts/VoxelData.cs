using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData {

    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 64;
    public static readonly int WorldSizeInChunks = 5;


    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize {
        get { return 1f / TextureAtlasSizeInBlocks; }
    }


    public static readonly Vector3Int[] faceChecks = {

        new Vector3Int(0, 0, -1), // Back Face
		new Vector3Int(0, 0, 1), // Front Face
		new Vector3Int(0, 1, 0), // Top Face
		new Vector3Int(0, -1, 0), // Bottom Face
		new Vector3Int(-1, 0, 0), // Left Face
		new Vector3Int(1, 0, 0), // Right Face

	};

    public static readonly Vector3[] voxelVerts = {

        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),

    };

    public static readonly int[,] voxelTris = {

        // 0 -> 1 -> 2 -> 2 -> 1 -> 3
        {0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6}, // Right Face

	};

    public static readonly Vector2[] voxelUvs = {

        new Vector2 (0.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (1.0f, 1.0f),

    };


}
