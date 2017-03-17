using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math {

    public static Vector3 RandomCircle(float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = 0;
        pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    public static Vector3 RandomCircle(float radius, Vector2 sector)
    {
        float ang = Random.Range(sector.x, sector.y);
        Vector3 pos;
        pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = 0;
        pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}
