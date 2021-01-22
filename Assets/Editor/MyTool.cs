using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MyTool : MonoBehaviour
{
    [MenuItem("MyTool/Clear")]
    static void DoSomething()
    {
       PlayerPrefs.DeleteAll();
    }
}
