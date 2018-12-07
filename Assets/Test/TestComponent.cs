using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    public void PrintString(string str)
    {
        Debug.Log(str);
    }

    public void PrintInt(int Int)
    {
        Debug.Log(Int);
    }

    public void PrintObjName(GameObject obj)
    {
        Debug.Log(obj.name);
    }

    public void PrintFloat(float fl)
    {
        Debug.Log(fl);
    }
}
