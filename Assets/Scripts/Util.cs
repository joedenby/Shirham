using UnityEngine;

public static class Util
{
    public static bool Approx(float a, float b, float tolerance) {
        var x = Mathf.Abs(a - b);
        Debug.Log(x);
        return x <= tolerance;
    } 
}
