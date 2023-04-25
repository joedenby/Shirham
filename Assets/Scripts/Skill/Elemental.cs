using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Elemental
{
    public ElementalType elementalType;
    public int value;


    public Elemental(ElementalType elementalType, int value) {
        this.elementalType = elementalType;
        this.value = value;
    }
    public override string ToString() => $"{elementalType}: {value}";
    public static string ToString(Elemental[] elementals)
    {
        string result = string.Empty;
        foreach (Elemental e in elementals) {
            result += $"{e}\n";
        }
        return result;
    }
    public static ElementalType Combine(ElementalType a, ElementalType b) {
        switch(a){
            case ElementalType.Fire:
                if (b.Equals(ElementalType.Water)) return ElementalType.Ground;
                if (b.Equals(ElementalType.Ice)) return ElementalType.Water;
                break;
            case ElementalType.Water:
                if (b.Equals(ElementalType.Ice)) return ElementalType.Ice;
                if (b.Equals(ElementalType.Electric)) return ElementalType.Electric;
                break;
            case ElementalType.Ice:
                if (b.Equals(ElementalType.Fire)) return ElementalType.Water;
                break;
            case ElementalType.Ground:
                if (b.Equals(ElementalType.Life)) return ElementalType.Life;
                if (b.Equals(ElementalType.Water)) return ElementalType.Water;
                break;
            case ElementalType.Electric:
                if (b.Equals(ElementalType.Ground)) return ElementalType.Ground;
                break;
            case ElementalType.Life:
                if (b.Equals(ElementalType.Rot)) return ElementalType.Rot;
                if (b.Equals(ElementalType.Fire)) return ElementalType.Fire;
                break;
            case ElementalType.Rot:
                if (b.Equals(ElementalType.Life)) return ElementalType.Life;
                break;
        }

        return a;
    }
    public int SetValue(int value) => value >= 0 ? (this.value = value) : 0;
    public static int TotalValue(Elemental[] elementals) => elementals.Sum(e => e.value);
    public static int SetValue(ElementalType elementalType, Elemental[] set, int value) {
        if (!Contains(elementalType, set)) return 0;
        Elemental e = Get(elementalType, set);
        e.SetValue(value);
        return e.value;
    }
    public static Elemental Get(ElementalType elementalType, Elemental[] elementals) {
        foreach (Elemental e in elementals) {
            if (e.elementalType == elementalType) 
                 return e;
        }

        return new Elemental(ElementalType.True, 0);
    }
    public static int GetValue(ElementalType elementalType, Elemental[] elementals) {
        foreach (Elemental e in elementals) { 
            if(e.elementalType == elementalType) 
                return e.value;
        }

        return 0;
    }
    public static Elemental[] NewElementalTable() => new Elemental[] {
        new Elemental(ElementalType.True, 0),
        new Elemental(ElementalType.Rot, 0),
        new Elemental(ElementalType.Strength, 0),
        new Elemental(ElementalType.Range, 0),
        new Elemental(ElementalType.Fire, 0),
        new Elemental(ElementalType.Water, 0),
        new Elemental(ElementalType.Ice, 0),
        new Elemental(ElementalType.Ground, 0),
        new Elemental(ElementalType.Electric, 0)
    };
    public static Elemental[] Sum(Elemental[] left, Elemental[] right) { 
        if(left.Length != right.Length) {
            Debug.LogError("Can't sum tables of varying length");
            return left;
        }

        for (int i = 0; i < left.Length; i++){
            left[i] = left[i] + right[i]; 
        }

        return left;
    }
    
    public static Elemental[] Neg(Elemental[] left, Elemental[] right)
    {
        if (left.Length != right.Length) {
            Debug.LogError("Can't sum tables of varying length");
            return left;
        }

        for (int i = 0; i < left.Length; i++) {
            if (left[i].elementalType != ElementalType.True)
                left[i] = left[i] - right[i];
        }

        return left;
    }
    public static Elemental[] Purge(Elemental[] elementals) {
        if (elementals == null || elementals.Length == 0) 
            return elementals;

        var selected = elementals.OrderByDescending(x => x.value);
        List<Elemental> returnTable = new List<Elemental>();
        foreach (Elemental elemental in selected) {
            if (elemental.value <= 0) return returnTable.ToArray();
            returnTable.Add(elemental);
        }

        return selected.ToArray();
    }
    public static bool TryParse(string query, out Elemental elemental) {
        string[] stringSplit = query.Split(" ");
        if (Enum.TryParse(typeof(ElementalType), stringSplit[0], true, out object x)) {
            int value = stringSplit.Length > 1 ? (int.TryParse(stringSplit[1], out int v) ? v : 0) : 0;
            elemental = new Elemental((ElementalType)x, value);
            return true;
        }

        elemental = null;
        return false;
    }
    public static bool Contains(ElementalType elementalType, Elemental[] set) {
        foreach (Elemental e in set) { 
            if(e.elementalType == elementalType)
                return true;
        }

        return false;
    }
    public static Elemental operator +(Elemental left, Elemental right) => 
        new Elemental(left.elementalType, (left.value + right.value));
    public static Elemental operator -(Elemental left, Elemental right) => 
        new Elemental(left.elementalType, (left.value - right.value));

}