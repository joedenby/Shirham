using System;

public struct Wallet
{
    public int Copper { get; private set; }
    public int Silver { get; private set; }
    public int Gold { get; private set; }
    public int asInt => (Gold*10000)+(Silver*100)+Copper;
   

    public Wallet(int c, int s, int g)
    {
        Copper = c;
        Silver = s;
        Gold = g;
        //Wallet X = new Wallet(4, 3, 5) + new Wallet (2 , 3 , 4);
    }

    public static Wallet IntToWallet(int value)
    {
        float g = (float)value / 10000;
        float x = MathF.Floor(g); 
        float r = g - x;
        g = x; //total gold
        float s = r * 100; 
        x = MathF.Floor(s);
        r = s - x;
        s = x; //total silver
        float c = r * 10;// total copper
        return new Wallet((int)g, (int)s, (int)c);

    }
    

    public static Wallet operator +(Wallet w, Wallet w2)
    {
        
        int c = (w.Copper + w2.Copper);
        int cRemainder = c % 100;
        int cTotal = (c - cRemainder) / 100;
        w.Copper = cRemainder; 

        int s = (w.Silver + w2.Silver);
        int sRemainder = s % 100;
        int sTotal = (s - sRemainder) / 100;
        w.Silver = cTotal + sRemainder;

        int g = (w.Gold + w2.Gold);
        w.Gold = g + sTotal;

        return new Wallet(w.Copper,w.Silver,w.Gold);
    }
    
    public static Wallet operator -(Wallet w, Wallet w2)
    {
        int wInt = w.asInt;
        int w2Int = w2.asInt;
        int w3Int = wInt - w2Int;

        return w3Int <= 0 ? new Wallet(0, 0, 0) : IntToWallet(w3Int); 
        
    }

    public override string ToString()
    {
        return $"G:{Gold} S:{Silver} C:{Copper}";
    }
}

