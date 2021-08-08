using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

    public ChunkCoord coord;

    World world;
    byte[,,] voxelGrid = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    PfVoxel[,,] PfVoxelGrid = new PfVoxel[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    Vector3Int chunkPosition;


    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    int vertexIndex;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();


    public Chunk(ChunkCoord coord, World world) {
        this.coord = coord;
        this.world = world;

        GameObject chunk = new GameObject("Chunk " + coord.x + " " + coord.z);
        chunkPosition = new Vector3Int(coord.x * VoxelData.ChunkWidth, 0, coord.z * VoxelData.ChunkWidth);
        chunk.transform.SetParent(world.transform);
        chunk.transform.position = chunkPosition;

        meshFilter = chunk.AddComponent<MeshFilter>();
        meshRenderer = chunk.AddComponent<MeshRenderer>();
        meshRenderer.material = world.chunkMaterial;

        PopulateVoxelGrid();
        CreateMeshData();
        CreateMesh();

    }

    private void PopulateVoxelGrid() {
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
            for (int z = 0; z < VoxelData.ChunkWidth; z++) {

                bool lastWasWalkable = false;

                for (int y = 0; y < VoxelData.ChunkHeight; y++) {
                    byte temp = world.GetFreshVoxel(new Vector3Int(x, y, z) + chunkPosition);
                    voxelGrid[x, y, z] = temp;

                    if (world.blocks[temp].isSolid) {
                        lastWasWalkable = true;
                        continue;
                    } else if (lastWasWalkable) {
                        PfVoxelGrid[x, y, z] = new PfVoxel(coord, x, y, z);
                    }

                    lastWasWalkable = false;
                }
            }
    }

    private void CreateMeshData() {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                    AddVoxelFacesToMeshData(new Vector3Int(x, y, z));

    }

    private void AddVoxelFacesToMeshData(Vector3Int position) {

        byte block = voxelGrid[position.x, position.y, position.z];

        if (!world.blocks[block].isSolid)
            return;
            

        for (int f = 0; f < 6; f++)
            if (!CheckForSolidVoxel(position + VoxelData.faceChecks[f])) {

                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[f, 0]] + position);
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[f, 1]] + position);
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[f, 2]] + position);
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[f, 3]] + position);

                AddTexture(world.blocks[block].textureId);

                triangles.Add(vertexIndex + 0);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4;
            }
    }

    private bool CheckForSolidVoxel(Vector3Int position) {

        if (position.y < 0 || position.y >= VoxelData.ChunkHeight)
            return false;
        else if (position.x >= 0 && position.x < VoxelData.ChunkWidth && position.z >= 0 && position.z < VoxelData.ChunkWidth)
            return world.blocks[voxelGrid[position.x, position.y, position.z]].isSolid;
        else
            return world.CheckForSolidVoxel(position + chunkPosition);
    }

    public byte GetVoxelFromGlobalPosition(Vector3Int position) {
        position.x -= chunkPosition.x;
        position.z -= chunkPosition.z;
        return voxelGrid[position.x, position.y, position.z];
    }

    private void CreateMesh() {
        Mesh mesh = new Mesh {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };
        //mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void AddTexture(int textureId) {

        float y = textureId / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureId - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        for (int i = 0; i < VoxelData.voxelUvs.Length; i++) {
            uvs.Add(VoxelData.voxelUvs[i] * VoxelData.NormalizedBlockTextureSize + new Vector2(x, y));
        }

    }

    #region Pathfinding

    public List<PfVoxel> GetNeighbours(PfVoxel voxel) {
        List<PfVoxel> neighbours = new List<PfVoxel>();

        for (int y = -1; y <= 1; y++)
            for (int x = -1; x <= 1; x++)
                for (int z = -1; z <= 1; z++) {
                    if (x == 0 && z == 0 || y != 0 && x != 0 && z != 0)
                        continue;

                    int checkX = voxel.gridX + x;
                    int checkY = voxel.gridY + y;
                    int checkZ = voxel.gridZ + z;

                    if (checkX >= 0 && checkX < VoxelData.ChunkWidth && checkY >= 0 && checkY < VoxelData.ChunkHeight && checkZ >= 0 && checkZ < VoxelData.ChunkWidth) {
                        if (PfVoxelGrid[checkX, checkY, checkZ] != null) {
                            neighbours.Add(PfVoxelGrid[checkX, checkY, checkZ]);
                        }
                    } else {
                        PfVoxel temp = world.SearchForPfVoxel(new Vector3Int(x, y, z) + chunkPosition);
                        if (temp != null) {
                            neighbours.Add(temp);
                        }
                    }
                }

         return neighbours;
    }

    public PfVoxel GetPfVoxelFromGlobalPosition(Vector3 position) {
        Vector3Int intPosition = new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z));

        intPosition.x -= chunkPosition.x;
        intPosition.z -= chunkPosition.z;
        return PfVoxelGrid[intPosition.x, intPosition.y, intPosition.z];
    }




    #endregion

}
