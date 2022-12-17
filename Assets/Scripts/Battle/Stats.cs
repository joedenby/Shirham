using UnityEngine;

[System.Serializable]
public struct Stats
{
    public int str;
    public int mag;
    public int end;

    public int Sum() => str + mag + end;
    public float StrAsPercent() => (float)str / Sum();
    public float MagAsPercent() => (float)mag / Sum();
    public float EndAsPercent() => (float)end / Sum();
    public int FinalStr() => Mathf.RoundToInt(str * StrAsPercent());
    public int FinalMag() => Mathf.RoundToInt(mag * MagAsPercent());
    public int FinalEnd() => Mathf.RoundToInt(end * EndAsPercent());
    public Stats Combined(Stats other)
    {
        return new Stats
        {
            str = str + other.str,
            mag = mag + other.mag,
            end = end + other.end
        };
    }
}
