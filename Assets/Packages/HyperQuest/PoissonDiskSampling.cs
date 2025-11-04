using System.Collections.Generic;
using UnityEngine;

public static class PoissonDiskSampling
{
    public struct PointWithMinDist
    {
        public Vector2 Point;
        public float MinDistance;

        public PointWithMinDist(Vector2 point, float minDistance)
        {
            Point = point;
            MinDistance = minDistance;
        }
    }

    public static List<Vector2> GeneratePoints(int maxPoints, Vector2 originPosition, float spawnRadius, float minDistRadius, List<PointWithMinDist> existingPointsWithDist = null, int numSamplesBeforeRejection = 30)
    {
        float cellSize = minDistRadius / Mathf.Sqrt(2);
        int gridDimension = Mathf.CeilToInt(spawnRadius * 2 / cellSize);
        int[,] grid = new int[gridDimension, gridDimension];
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(originPosition); // Start from the origin position

        while (spawnPoints.Count > 0 && points.Count < maxPoints)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + dir * Random.Range(minDistRadius, 2 * spawnRadius);
                if (IsValid(candidate, spawnRadius, originPosition, cellSize, minDistRadius, points, grid, existingPointsWithDist))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    int gridX = Mathf.FloorToInt((candidate.x - originPosition.x + spawnRadius) / cellSize);
                    int gridY = Mathf.FloorToInt((candidate.y - originPosition.y + spawnRadius) / cellSize);
                    grid[gridX, gridY] = points.Count; // Mark grid cell as occupied
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        return points;
    }

    private static bool IsValid(Vector2 candidate, float spawnRadius, Vector2 originPosition, float cellSize, float minDistRadius, List<Vector2> points, int[,] grid, List<PointWithMinDist> existingPointsWithDist)
    {
        Vector2 relativePos = candidate - originPosition;
        if (relativePos.magnitude > spawnRadius)
        {
            return false;
        }

        int gridX = Mathf.FloorToInt((candidate.x - originPosition.x + spawnRadius) / cellSize);
        int gridY = Mathf.FloorToInt((candidate.y - originPosition.y + spawnRadius) / cellSize);
        int searchStartX = Mathf.Max(0, gridX - 2);
        int searchEndX = Mathf.Min(gridX + 2, grid.GetLength(0) - 1);
        int searchStartY = Mathf.Max(0, gridY - 2);
        int searchEndY = Mathf.Min(gridY + 2, grid.GetLength(1) - 1);

        for (int x = searchStartX; x <= searchEndX; x++)
        {
            for (int y = searchStartY; y <= searchEndY; y++)
            {
                int pointIndex = grid[x, y] - 1;
                if (pointIndex != -1)
                {
                    float sqrDist = (candidate - points[pointIndex]).sqrMagnitude;
                    if (sqrDist < minDistRadius * minDistRadius)
                    {
                        return false;
                    }
                }
            }
        }

        if (existingPointsWithDist != null)
        {
            foreach (var pointWithDist in existingPointsWithDist)
            {
                if ((pointWithDist.Point - candidate).sqrMagnitude < pointWithDist.MinDistance * pointWithDist.MinDistance)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
