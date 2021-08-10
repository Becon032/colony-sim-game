using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    public int seed;

    public Material chunkMaterial;

    public Chunk[,] chunks = new Chunk[VoxelData.WorldSizeInChunks, VoxelData.WorldSizeInChunks];
    public Block[] blocks;

    private void Start() {

        Random.InitState(seed);

        GenerateWorld();
    }

    private void GenerateWorld() {
        for (int x = 0; x < VoxelData.WorldSizeInChunks; x++) {
            for (int z = 0; z < VoxelData.WorldSizeInChunks; z++) {
                chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
            }
        }
    }

    public byte GetFreshVoxel(Vector3Int pos) {

        int yPos = Mathf.FloorToInt(pos.y);

        /* */
        //If bottom of chunk

        if (yPos == 0)
            return 1;

        /* BASIC TERRAIN PASS */

        int terrainHeight = Mathf.FloorToInt(VoxelData.ChunkHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 500, .25f));
        byte voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = 0;
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = 0;
        else if (yPos > terrainHeight)
            return 2;
        else
            voxelValue = 1;


        return voxelValue;
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


