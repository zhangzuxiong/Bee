using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class SaveMesh : MonoBehaviour
{
    public bool saveMesh=false;

    void Update()
    {
        if (saveMesh)
            SaveAsset();
    }
    void SaveAsset()
    {
        saveMesh = false;
#if UNITY_EDITOR
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        AssetDatabase.CreateAsset(mesh, "Assets/" + gameObject.name + ".asset");
#endif
    }
}
