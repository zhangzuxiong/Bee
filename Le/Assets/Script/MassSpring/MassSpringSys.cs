using UnityEngine;

public class MassSpringSys : MonoBehaviour
{
    private Material mat;
    private Vector3 followPos = Vector3.zero;//从动点位置
    private Vector3 massVelocity = Vector3.zero;//从动点速度
    public float stiffness = 600f;//劲度系数
    public float damping = 10f;//阻尼系数

    private float max, min;//模型在模型空间最高, 最低点的y值
    private void Start()
    {
        mat = GetComponent<MeshRenderer>().sharedMaterial;
        followPos = transform.position;//虚拟抽象一个从动点

        //距离轴心的物体空间距离
        max = GetComponent<MeshFilter>().sharedMesh.bounds.max.y;
        min = GetComponent<MeshFilter>().sharedMesh.bounds.min.y;

        mat.SetFloat("_MeshH", max - min);//模型总高度
    }
    private void Update()
    {
        //进行一些受力, 加速度, 速度, 路程, 运动 的数值计算
        Vector3 force = GetMainForce();//弹力
        force += GetDampingForce();//阻力
        massVelocity += force * Time.deltaTime;//将固定质量为1, 则force数值等于加速度数值
        followPos += massVelocity * Time.deltaTime;//从动点的移动

        //为shader传入数据
        SetMatData();
    }
    private Vector3 GetMainForce()
    {
        //胡克定律
        Vector3 forceDir = transform.position - followPos;
        return forceDir * stiffness;
    }
    private Vector3 GetDampingForce()
    {
        return -massVelocity * damping;//弹簧阻尼
    }
    private void SetMatData()
    {
        mat.SetVector("_MainPos", transform.position);//主动点
        mat.SetVector("_FollowPos", followPos);//从动点
        mat.SetFloat("_W_Bottom", transform.position.y + min);//模型最低点y值
    }
}