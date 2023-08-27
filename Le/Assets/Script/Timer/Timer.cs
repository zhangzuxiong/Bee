using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bee
{
    public class Timer : IDisposable
    {
        static int _id = 1;
        
        /// <summary>
        /// ID
        /// </summary>
        public int ID = _id++;
        private int _group = 0;
        /// <summary>
        /// 延迟执行
        /// </summary>
        private float _delay = 0.0f;
        /// <summary>
        /// 等待条件成立执行
        /// </summary>
        private Func<bool> _waitConFunc;
        /// <summary>
        /// 重复执行的时间间隔
        /// </summary>
        private float _loopInterval = 0.0f;
        /// <summary>
        /// 重复执行的次数，-1表示一直执行
        /// </summary>
        private int _loopTimes = 1;
        /// <summary>
        /// 执行的Action
        /// </summary>
        private Action _action;
        /// <summary>
        /// 是否暂停
        /// </summary>
        private bool isPause = true;
        /// <summary>
        /// 是否执行完成
        /// </summary>
        private bool isCompleted = false;

        /// <summary>
        /// 所属组，默认是0
        /// </summary>
        public int Group => _group;

        internal  static Timer CreateTimer(float delay, Action action, Func<bool> waitConFunc = null, int loopTimes = 1,
            float loopInterval = 0, int group = 0)
        {
            Timer timer = new Timer();
            timer._delay = delay;
            timer._waitConFunc = waitConFunc;
            timer._action = action;
            timer._loopTimes = loopTimes;
            timer._loopInterval = loopInterval;
            timer._group = group;
            return timer;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        /// <returns></returns>
        public Timer Pause()
        {
            isPause = true;
            return this;
        }

        /// <summary>
        /// 恢复
        /// </summary>
        /// <returns></returns>
        public Timer Resume()
        {
            isPause = false;
            return this;
        }

        /// <summary>
        /// 添加等待条件
        /// </summary>
        /// <param name="waitConFunc">条件</param>
        /// <returns></returns>
        public Timer AddWaitCondition(Func<bool> waitConFunc)
        {
            _waitConFunc = waitConFunc;
            return this;
        }
        
        /// <summary>
        /// 分组
        /// </summary>
        /// <param name="group">分给那个组</param>
        /// <returns></returns>
        public Timer AddGroup(int group)
        {
            _group = group;
            return this;
        }

        public Timer Kill()
        {
            isPause = true;
            isCompleted = true;
            return this;
        }
        
        /// <summary>
        /// 开始执行
        /// </summary>
        /// <returns></returns>
        public Timer Execute()
        {
            isPause = false;
            isCompleted = false;
            return this;
        }

        public bool CheckCompleted()
        {
            return isCompleted;
        }

        internal void Update()
        {
            if(isCompleted) return;
            if(isPause) return;
            if (_loopTimes == 0)
            {
                isCompleted = true;
                return;
            }

            if (_waitConFunc != null)
            {
                if (_waitConFunc.Invoke()) _waitConFunc = null;
                return;
            }

            if (_delay >= 0)
            {
                _delay -= Time.deltaTime;
                return;
            }
            if (_action != null)
            {
                _action.Invoke();
                if (_loopTimes == -1)
                {
                    _delay = _loopInterval;
                }
                else
                {
                    _loopTimes--;
                    if (_loopTimes <= 0)
                    {
                        _action = null;
                        isCompleted = true;
                    }
                    else
                    {
                        _delay = _loopInterval;
                    }
                }
            }
            else
            {
                isCompleted = true;
            }
        }

        public void Dispose()
        {
            _waitConFunc = null;
            _action = null;
            isPause = true;
        }
    }

    public class TimerManager : MonoSingleton<TimerManager>
    {
        protected override bool IsDontDestroy => true;
        private List<Timer> timers = new List<Timer>();
        
        /// <summary>
        /// 延迟执行一次
        /// </summary>
        /// <param name="delay">延迟时间，单位秒</param>
        /// <param name="action">延迟执行的Action</param>
        /// <returns></returns>
        public Timer Once(float delay,Action action)
        {
            Timer timer = Timer.CreateTimer(delay, action);
            timers.Add(timer);
            return timer;
        }

        /// <summary>
        /// 重复执行
        /// </summary>
        /// <param name="delay">首次执行的延迟时间，单位秒</param>
        /// <param name="action">重复执行的Action</param>
        /// <param name="loopTimes">重复执行的次数，-1表示一直重复执行</param>
        /// <param name="loopInterval">重复执行的间隔，单位秒</param>
        /// <returns></returns>
        public Timer Loop(float delay,Action action,int loopTimes,float loopInterval)
        {
            Timer timer = Timer.CreateTimer(delay, action, null, loopTimes, loopInterval);
            timers.Add(timer);
            return timer;
        }

        /// <summary>
        /// 删除Timer
        /// </summary>
        /// <param name="id">ID</param>
        public void ClearTimerByID(int id)
        {
            for (int i = 0; i < timers.Count; i++)
            {
                if (timers[i].ID == id)
                {
                    timers.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// 删除所属组的Timer
        /// </summary>
        /// <param name="group">组</param>
        public void ClearTimerByGroup(int group)
        {
            for (int i = timers.Count - 1; i >= 0; i--)
            {
                if(timers[i].Group == group) timers.RemoveAt(i);
            }
        }
        
        public void Update()
        {
            for(int i = 0;i < timers.Count;i++) timers[i].Update();

            for (int i = timers.Count - 1; i >= 0; i--)
            {
                if (timers[i].CheckCompleted())
                    timers.RemoveAt(i);
            }
        }
    }
}