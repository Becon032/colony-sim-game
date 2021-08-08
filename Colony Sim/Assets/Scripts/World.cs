using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public Material chunkMaterial;

    public Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    public Block[] blocks;

    private void Start() {
        GenerateWorld();
    }

    private void GenerateWorld() {
        for (int x = 0; x < VoxelData.WorldSizeInChunks; x++) {
            for (int z = 0; z < VoxelData.WorldSizeInChunks; z++) {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
            }
        }
    }

    public byte GetFreshVoxel(Vector3Int voxelPosition) {

        if (voxelPosition.y < VoxelData.ChunkHeight / 3)
            return 1;
        else if (voxelPosition.y < VoxelData.ChunkHeight / 2)
            return 0;
        else
            return 2;

    }

    public bool CheckForSolidVoxel(Vector3Int position) {

        int x = Mathf.FloorToInt((float)position.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt((float)position.z / VoxelData.ChunkWidth);

        ChunkCoord testingChunk = new ChunkCoord(x, z);

        if (position.y < 0 || position.y >= VoxelData.ChunkHeight || !IsChunkInWorld(testingChunk))
            return false;

        if (chunks[testingChunk.x, testingChunk.z] != null)
            return blocks[chunks[testingChunk.x, testingChunk.z].GetVoxelFromGlobalPosition(position)].isSolid;

        return blocks[GetFreshVoxel(position)].isSolid;
    }

    private bool IsChunkInWorld(ChunkCoord coord) {
        if (coord.x < 0 || coord.x >= VoxelData.WorldSizeInChunks || coord.z < 0 || coord.z >= VoxelData.WorldSizeInChunks)
            return false;
        else
            return true;
    }

    public PfVoxel SearchForPfVoxel(Vector3 position) {

        int x = Mathf.FloorToInt((float)position.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt((float)position.z / VoxelData.ChunkWidth);

        ChunkCoord testingChunk = new ChunkCoord(x, z);

        if (position.y < 0 || position.y >= VoxelData.ChunkHeight || !IsChunkInWorld(testingChunk))
            return null;

        return chunks[testingChunk.x, testingChunk.z].GetPfVoxelFromGlobalPosition(position);
    }


}

[System.Serializable]
public struct Block {
    public string name;
    public bool isSolid;
    public int textureId;
}


