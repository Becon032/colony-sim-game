using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PfVoxel {

    public Vector3 worldPosition;
    public ChunkCoord chunkCoord;
    public int gridX;
    public int gridY;
    public int gridZ;

    public PfVoxel parent;
    public int gCost;
    public int hCost;

    public PfVoxel(ChunkCoord chunkCoord, int gridX, int gridY, int gridZ) {
        this.chunkCoord = chunkCoord;
        this.gridX = gridX;
        this.gridY = gridY;
        this.gridZ = gridZ;
    }

    public int fCost {
        get {
            return gCost + hCost;
        }
    }

}
