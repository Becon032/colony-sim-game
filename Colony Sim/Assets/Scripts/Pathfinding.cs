using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public Transform seeker, target;
    World world;

    private void Awake() {
        world = GetComponent<World>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            FindPath(seeker.position, target.position);
    }

    private void FindPath(Vector3 startPos, Vector3 targetPos) {

        PfVoxel startVoxel = world.SearchForPfVoxel(startPos);
        PfVoxel targetVoxel = world.SearchForPfVoxel(targetPos);

        List<PfVoxel> openSet = new List<PfVoxel>();
        HashSet<PfVoxel> closedSet = new HashSet<PfVoxel>();
        openSet.Add(startVoxel);

        while (openSet.Count > 0) {
            PfVoxel currentVoxel = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentVoxel.fCost || openSet[i].fCost == currentVoxel.fCost && openSet[i].hCost < currentVoxel.hCost) {
                    currentVoxel = openSet[i];
                }
            }

            openSet.Remove(currentVoxel);
            closedSet.Add(currentVoxel);

            if (currentVoxel == targetVoxel) {
                RetracePath(startVoxel, targetVoxel);
                return;
            }

            foreach (PfVoxel neighbour in world.chunks[currentVoxel.chunkCoord.x, currentVoxel.chunkCoord.z].GetNeighbours(currentVoxel)) {

                if (closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentVoxel.gCost + GetDistance(currentVoxel, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetVoxel);

                    neighbour.parent = currentVoxel;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    private void RetracePath(PfVoxel startVoxel, PfVoxel endVoxel) {
        List<PfVoxel> path = new List<PfVoxel>();
        PfVoxel currentVoxel = endVoxel;

        while (currentVoxel != startVoxel) {
            path.Add(currentVoxel);
            currentVoxel = currentVoxel.parent;
        }
        path.Reverse();

        Debug.Log("znaleziono drogę!");
    }

    private int GetDistance(PfVoxel voxelA, PfVoxel voxelB) {
        int distanceX = Mathf.Abs(voxelA.gridX - voxelB.gridX) + VoxelData.ChunkWidth * Mathf.Abs(voxelA.chunkCoord.x - voxelB.chunkCoord.x);
        int distanceY = Mathf.Abs(voxelA.gridY - voxelB.gridY);
        int distanceZ = Mathf.Abs(voxelA.gridZ - voxelB.gridZ) + VoxelData.ChunkWidth * Mathf.Abs(voxelA.chunkCoord.z - voxelB.chunkCoord.z);

        if (distanceX > distanceZ)
            return 14 * distanceZ + 10 * (distanceX - distanceZ) + 4 * distanceY;
        else
            return 14 * distanceX + 10 * (distanceZ - distanceX) + 4 * distanceY;
    }


}
