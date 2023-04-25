using Language.Lua;
using UnityEngine;

[System.Serializable]
public struct Stats
{
    public int str;
    public int mag;
    public int end;

    public int Sum => str + mag + end;

    public float StrAsPercent => (float)str / Sum;
    public float MagAsPercent => (float)mag / Sum;
    public float EndAsPercent => (float)end / Sum;

    public int RelativeStr => Mathf.RoundToInt(str * StrAsPercent);
    public int RelativeMag => Mathf.RoundToInt(mag * MagAsPercent);
    public int RelativeEnd => Mathf.RoundToInt(end * EndAsPercent);


    public Stats(int str = 0, int mag = 0, int end = 0)
    {
        this.str = str;
        this.mag = mag;
        this.end = end;
    }

    public static Stats operator +(Stats a, Stats b) {
        return new Stats
        {
            str = a.str + b.str,
            mag = a.mag + b.mag,
            end = a.end + b.end
        };
    }
    public static Stats operator -(Stats a, Stats b)
    {
        return new Stats
        {
            str = a.str - b.str,
            mag = a.mag - b.mag,
            end = a.end - b.end
        };
    }
    public static Stats operator *(Stats a, Stats b)
    {
        return new Stats
        {
            str = a.str * b.str,
            mag = a.mag * b.mag,
            end = a.end * b.end
        };
    }
    public static Stats operator /(Stats a, Stats b)
    {
        return new Stats
        {
            str = a.str / b.str,
            mag = a.mag / b.mag,
            end = a.end / b.end
        };
    }
}
