namespace SAA_MDDB;

struct Column
{
    private string _default = "NULL";
    private bool _autoIncrement = false;

    public string DefaultValue
    {
        get => _default;
        set
        {
            if (_default != value)
                _default = value;
        }
    }
    public bool IsAutoIncrement
    {
        get => _autoIncrement;
        set
        {
            if (Type == MDDBType.Int)
                _autoIncrement = value;
        }
    }

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

    public override string? ToString() => $"|{StringHelper.AddPadding(20, Name)}" +
        $"{StringHelper.AddPadding(20, Type.ToString())}{StringHelper.AddPadding(20, _default)}" +
        $"{StringHelper.AddPadding(20, _autoIncrement ? "True" : "False")}";
}