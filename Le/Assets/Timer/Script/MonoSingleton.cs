using System.Threading;
using UnityEngine;

namespace Bee
{
    public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
    {
        /// <summary>
        /// 跨场景是否不销毁
        /// </summary>
        protected abstract bool IsDontDestroy { get; }
        /// <summary>
        /// 是否已初始化
        /// </summary>
        private bool _IsInited;

        /// <summary>
        /// 锁
        /// </summary>
        private static int _Lock;

        /// <summary>
        /// 单例
        /// </summary>
        private static T _Instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<T>();
                    if (_Instance == null)
                    {
                        // 以原子操作的形式，将 32 位有符号整数设置为指定的值并返回原始值。用于代替线程锁
                        if (0 == Interlocked.Exchange(ref _Lock, 1))
                        {
                            try
                            {
                                if (_Instance == null)
                                {
                                    _Instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                                    _Instance.DoInit();
                                }
                            }
                            finally
                            {
                                Interlocked.Exchange(ref _Lock, 0);
                            }
                        }
                    }
                }
                return _Instance;
            }
        }

        protected virtual void Awake()
        {
            if (!_Instance)
            {
                _Instance = this as T;
                DoInit();
            }
            else if (_Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void DoInit()
        {
            if (!_IsInited)
            {
                _IsInited = true;
                if (IsDontDestroy)
                {
                    DontDestroyOnLoad(gameObject);
                }
                OnInit();
            }
        }

        public virtual void Dispose()
        {
            if (this == _Instance)
            {
                _Instance = null;
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (this == _Instance)
            {
                _Instance = null;
            }
        }

        /// <summary>
        /// do something after init
        /// </summary>
        public virtual void OnInit()
        {

        }
    }

}
