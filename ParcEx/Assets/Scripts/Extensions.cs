using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions {
    public static Vector3 GetPointBetweenTwoVectors(Vector3 start, Vector3 end, float percent)
    {
        return ((1 - percent) * (start) + percent * end);
    }
}
