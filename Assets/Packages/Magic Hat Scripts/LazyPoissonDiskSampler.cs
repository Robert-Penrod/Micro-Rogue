using System.Collections.Generic;
using UnityEngine;

public static class LazyPoissonDiscSampler
{
    public static List<Vector2> GetPoints(Vector2 pos, float radius, int maxPointCount, float minDist, List<List<MinDistPoint>> priorPoints = null, int maxIter = 30)
    {
        List<Vector2> pointList = new List<Vector2>();

        float sqrMinDist = minDist * minDist;

        for(int tryCount = 0; tryCount < maxIter && pointList.Count < maxPointCount; tryCount++)
        {
            Vector2 randomPoint = pos + Random.insideUnitCircle * radius;

            // Check dist
            bool isValid = true;
            foreach (Vector2 p in pointList)
            {
                float sqrDist = (p - randomPoint).sqrMagnitude;
                if (sqrDist < sqrMinDist)
                {
                    isValid = false;
                    break;
                }
            }
            if (!isValid) continue;

            if(priorPoints != null)
            {
                foreach (List<MinDistPoint> minDistMap in priorPoints)
                {
                    foreach (MinDistPoint minDistPoint in minDistMap)
                    {
                        float sqrDist = (minDistPoint.Point - randomPoint).sqrMagnitude;
                        if (sqrDist < minDistPoint.MinDistance * minDistPoint.MinDistance)
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
            }
            if (!isValid) continue;

            // Found valid point
            tryCount--;
            pointList.Add(randomPoint);
        }

        return pointList;
    }

    public struct MinDistPoint
    {
        public Vector2 Point;
        public float MinDistance;

        public MinDistPoint(Vector2 point, float minDistance)
        {
            Point = point;
            MinDistance = minDistance;
        }
    }
}
