using System;
using UnityEngine;

namespace Bee.Test
{
    public class SingletonTest : MonoSingleton<SingletonTest>
    {
        protected override bool IsDontDestroy => true;

        public void Test()
        {
            Debug.Log("这是一个单例");
        }
    }
    public class Test : MonoBehaviour
    {
        //单例测试
        [RuntimeInitializeOnLoadMethod]
        static void _SingletonTest()
        {
            SingletonTest.Instance.Test();
        }
        
        
        //Timer测试
        [RuntimeInitializeOnLoadMethod]
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
    }
}