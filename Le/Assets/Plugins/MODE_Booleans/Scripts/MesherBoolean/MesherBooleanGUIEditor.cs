using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MainMesherBoolean))]
public class MesherBooleanGUIEditor : EditorWindow
{
    /// <summary>
    /// 得到正在编辑类
    /// </summary>
    MainMesherBoolean MesherBoolean;

    private static MesherBooleanGUIEditor _window;
    [MenuItem("Tools/CSG/控制窗口")]
    public static void GUIDRefReplaceWin()
    {
        Rect wr = new Rect(0, 0, 300, 250);
        // true 表示不能停靠的
        _window = (MesherBooleanGUIEditor)GetWindowWithRect(typeof(MesherBooleanGUIEditor), wr, true, "布尔运算");
        _window.Show();
    }
    void OnGUI()
    {
        if (MesherBoolean == null)
        {
            Debug.Log("null");
            MesherBoolean = new MainMesherBoolean();
        }
        GUILayout.Space(20);
        MesherBoolean.Target = (GameObject)EditorGUILayout.ObjectField("被减物体", MesherBoolean.Target, typeof(GameObject), true);
        //EditorGUILayout.ObjectField("被减物体", MesherBoolean.Target, typeof(System.Object), true);
        MesherBoolean.Brush = (GameObject)EditorGUILayout.ObjectField("物体", MesherBoolean.Brush, typeof(GameObject), true);

        if (GUILayout.Button("正常切割"))
        {
            MesherBoolean.GetBooleanObj(BooleanType.正常切割).AddComponent<SaveMesh>();
        }
        if (GUILayout.Button("交叉"))
        {
            MesherBoolean.GetBooleanObj(BooleanType.交叉).AddComponent<SaveMesh>();
        }
        if (GUILayout.Button("合并"))
        {
            MesherBoolean.GetBooleanObj(BooleanType.合并).AddComponent<SaveMesh>();
        }
    }
}
