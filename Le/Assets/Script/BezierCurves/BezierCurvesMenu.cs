using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEditor;
using UnityEngine;

public class BezierCurvesMenu : MonoBehaviour
{
    [MenuItem("GameObject/Create Other/创建一条曲线")]
    public static void CreateBezierCurves()
    {
        var go = Selection.activeGameObject;
        var nGo = new GameObject("Curves");
        nGo.transform.position = Vector3.zero;
        if (go != null)
            nGo.transform.SetParent(go.transform, true);
        nGo.AddComponent<PathCreator>();
    }
}