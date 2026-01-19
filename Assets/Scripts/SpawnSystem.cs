using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpawnSystem
{
    public static Vector2 GetRandomEmptyPosAvoidingCircle(Vector2 origin, float emptyRadius, Vector2 avoidanceOrigin, float avoidanceRadius)
    {
        for(int maxIter = 100; maxIter > 0; maxIter--)
        {
            Vector2 pos = GetRandomEmptyPos(origin, emptyRadius);
            float dist = Vector2.Distance(pos, avoidanceOrigin);
            if (dist > avoidanceRadius) return pos;
        }
        Debug.LogWarning("Ran out of iterations finding avoidance point");
        return new Vector2(0, 10000);
    }

    public static Vector2 GetRandomEmptyPos(Vector2 origin, float emptyRadius = 0.75f) => GetRandomEmptyPos(emptyRadius, origin.x, origin.y);

    public static Vector2 GetRandomEmptyPos(float emptyRadius = 0.75f, float originX = 0, float originY = 0)
    {
        if(emptyRadius == 0)
        {
            return GetRandomPoint();
        }

        bool isEmpty = false;
        Vector2 pos = new Vector2();
        for (int maxIter = 500; maxIter > 0 && !isEmpty; maxIter--)
        {
            isEmpty = true;
            pos = new Vector2(originX, originY) + GetRandomPoint();
            var prevTriggerHit = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = true;
            Collider2D[] colArray = Physics2D.OverlapCircleAll(pos, emptyRadius, LayerMask.GetMask("Default"));
            Physics2D.queriesHitTriggers = prevTriggerHit;
            for (int i = 0; i < colArray.Length; i++)
            {
                if (!colArray[i].isTrigger)
                {
                    isEmpty = false;
                    break;
                }
            }
        }
        return pos;
    }

    public static Vector2 GetRandomPosNearWall(float emptyRadius = 0.15f, float wallMinDist = 0.5f, float wallMaxDist = 1.5f)
    {
        bool foundPos = false;
        Vector2 pos = new Vector2();
        for(int maxIter = 1000; maxIter > 0 && !foundPos; maxIter--)
        {
            pos = GetRandomEmptyPos(emptyRadius);

            // Check wall Proximity
            Collider2D[] colArray = Physics2D.OverlapCircleAll(pos, wallMaxDist);
            for(int i = 0; i < colArray.Length; i++)
            {
                // Check if collider is wall
                if(colArray[i].gameObject.layer == 6)
                {
                    Vector2 closestWallPoint = colArray[i].ClosestPoint(pos);
                    float wallDist = (closestWallPoint - pos).magnitude;
                    if(wallDist >= wallMinDist && wallMinDist <= wallMaxDist)
                    {
                        return pos;
                    }
                }
            }
        }

        return pos;
    }

    static Vector2 GetRandomPoint()
    {
        return Random.insideUnitCircle * 20f;
    }
}
