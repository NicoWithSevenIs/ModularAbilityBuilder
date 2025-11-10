using System;

[Serializable]
public struct Stat
{
    public uint HP;
    public uint ATK;
    public uint SPA;
    public uint DEF;
    public uint SPD;
    public uint SPE;

    public Stat(uint[] l)
    {
        HP = l[0];
        ATK = l[1];
        DEF = l[2];
        SPA = l[3];
        SPD = l[4];
        SPE = l[5];
    }

}
