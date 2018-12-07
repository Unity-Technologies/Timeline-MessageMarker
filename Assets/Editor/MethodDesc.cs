using System;

class MethodDesc
{
    public string name;
    public ParameterType type;
    public bool isOverload;
    public override string ToString()
    {
        return string.Format("{0}({1})", name, ParameterTypeToString());
    }

    public string ParameterTypeToString()
    {
        switch (type)
        {
            case ParameterType.Int:
                return "int";
            case ParameterType.Float:
                return "float";
            case ParameterType.Object:
                return "Object";
            case ParameterType.String:
                return "string";
            default:
                return string.Empty;
        }
    }
}