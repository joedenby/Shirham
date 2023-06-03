using UnityEngine;

public static class Util
{
    public static bool Approx(float a, float b, float tolerance) {
        var x = Mathf.Abs(a - b);
        Debug.Log(x);
        return x <= tolerance;
    }

    public static Transform GetChild(Transform transform, params int[] indexs) {
        int i = 0;
        for (i = 0; i < indexs.Length; i++) {
            if (i == (indexs.Length - 1)) {
                return indexs[i] < transform.childCount ? transform.GetChild(indexs[i]) : null;
            }

            if (indexs[i] == -1) 
                continue;

            if (indexs[i] >= transform.childCount) {
                Debug.LogError($"Transform '{transform.name}' does not contain child count of {indexs[i]}");
                return null;
            }

            transform = transform.GetChild(indexs[i]);
            indexs[i] = -1;
            break;
        }

        return GetChild(transform, indexs);
    }

    public static bool InBounds(int value, int lowerBound, int upperBound) { 
        return value >= lowerBound && value <= upperBound;
    }

    public static bool InBounds(float value, float lowerBound, float upperBound) { 
        return value >= lowerBound && value <= upperBound;
    }

    public static float ForceNormalize(float value) { 
        return value < 0 ? -1 : 1;
    }

    public static Vector2 Triangulate(Vector2 a, Vector2 b, Vector2 c)
    {
        float centerX = (a.x + b.x + c.x) / 3;
        float centerY = (a.y + b.y + c.y) / 3;

        return new Vector2(centerX, centerY);
    }

}
