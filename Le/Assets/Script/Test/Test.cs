using System;
using UnityEditor;
using UnityEngine;

namespace Bee.Test
{
    [Serializable]
    public class Student
    {
        public string name;
        public int age;
        public override string ToString()
        {
            return $"age:{age},name{name}";
        }
    }

    public class JSONTest
    {
        [MenuItem("Test/json")]
        public static void Test()
        {
            Tools.ClearConsole();
            Student stu = new Student() { age = 20, name = "Tom" };
            string s = JsonUtility.ToJson(stu);
            Debug.LogError(s);
            Student student = JsonUtility.FromJson<Student>(s);
            Debug.LogError(student.ToString());
        }
    }
    public class SingletonTest : MonoSingleton<SingletonTest>
    {
        protected override bool IsDontDestroy => true;

        public void Test()
        {
            Debug.Log("这是一个单例");
        }
    }

    public class PoolItem
    {
        public int id;
        public PoolItem(int id)
        {
            this.id = id;
        }
    }
    
    public class Test : MonoBehaviour
    {
        //单例测试
        // [RuntimeInitializeOnLoadMethod]
        static void _SingletonTest()
        {
            SingletonTest.Instance.Test();
        }
        
        
        //Timer测试
        // [RuntimeInitializeOnLoadMethod]
        static void TimerTest()
        {
            bool condition = false;
            Func<bool> conFunc = () => condition;
            Timer once = TimerManager.Instance.Once(5, () => { 
                Debug.LogError("延迟5s执行");
                condition = true;
            }).Execute();

            Timer conTimer = TimerManager.Instance.Once(5, () =>
            {
                Debug.LogError("等待条件成立");
            }).AddWaitCondition(conFunc).Execute();

            // Timer loop = TimerManager.Instance.Loop(0, () => { Debug.LogError("重复执行,执行5次"); }, 5, 2).Execute();

            Timer loop_ = TimerManager.Instance.Loop(0, () => { Debug.LogError("重复执行,一直执行"); }, -1, 1).Execute();

            TimerManager.Instance.Once(10, () =>
            {
                // TimerManager.Instance.ClearTimerByGroup(0);
                // TimerManager.Instance.ClearTimerByID(loop_.ID);
                loop_.Pause();
                loop_.Kill();
            }).Execute();
        }


        [RuntimeInitializeOnLoadMethod]
        static void PoolTest()
        {
            int id = 1;
            Pool<PoolItem> pool = PoolMgr.CreatePool("PoolItem", () =>
            {
                return new PoolItem(id++);
            });

            PoolItem item = pool.GetObjFromPool();
            Debug.Log(item.id);
        }
    }
}