namespace SAA_MDDB;

readonly struct Column
{
    public string Name { get; }
    public MDDBType Type { get; }

    public Column(string name, MDDBType type)
    {
        Name = name;
        Type = type;
    }

    public int GetSize()
    {
        switch (Type)
        {
            case MDDBType.Int: return 4;
            case MDDBType.Date: return 255;
            case MDDBType.String: return 255;
        }
        return 0;
    }
}