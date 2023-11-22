using MeshMakerNamespace;
using UnityEngine;
public enum BooleanType
{
    正常切割,
    交叉,
    合并
}
public class MainMesherBoolean
{
    [Header("被减物体")]
    public GameObject Target;
    [Header("物体")]
    public GameObject Brush;
    [Header("CSG 类型")]
    public BooleanType booleanType;
    
    CSG CsgObj;

    /// <summary>
    /// 设置类型，创建模型
    /// </summary>
    /// <param name="operation">
    /// 
    /// CSG.Operation.Subtract = 正常切割
    /// CSG.Operation.Union = 合并
    /// CSG.Operation.Intersection = 得到交叉部分
    /// 
    /// </param>
    /// <returns>得到创建好的模型</returns>
    GameObject CreateModel(CSG.Operation operation)
    {
        CsgObj = GetCsg(operation);
        return CsgObj.PerformCSG();
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        GetBooleanObj(booleanType).name = "" + booleanType;
    //    }
    //}

    /// <summary>
    /// 得到布尔物体
    /// </summary>
    /// <param name="booleanType">布尔类型</param>
    /// <returns></returns>
    public GameObject GetBooleanObj(BooleanType booleanType)
    {
        GameObject BooleanObj = null;
        if (Target==null|| Brush==null)
        {
            Debug.Log("缺少目标,请确认布尔物体是否存在");
            return null;
        }
        switch (booleanType)
        {
            case BooleanType.正常切割:
                BooleanObj = CreateModel(CSG.Operation.Subtract);
                break;
            case BooleanType.交叉:
                BooleanObj = CreateModel(CSG.Operation.Intersection);
                break;
            case BooleanType.合并:
                BooleanObj = CreateModel(CSG.Operation.Union);
                break;
        }
        return BooleanObj;
    }

    /// <summary>
    /// 保存CSG的参数
    /// </summary>
    /// <param name="OperationType"></param>
    /// <returns></returns>
    CSG GetCsg(CSG.Operation OperationType)
    {
        CSG csg;
        csg = new CSG();
        csg.Brush = Brush;
        csg.Target = Target;
        csg.OperationType = OperationType;
        csg.customMaterial = new Material(Shader.Find("Standard")); // 材质
        csg.useCustomMaterial = false; // 使用上面的材质来填充切口
        csg.hideGameObjects = true; // 操作后隐藏目标和画笔对象
        csg.keepSubmeshes = true; // 保持原始的网格和材质
        return csg;
    }
}
