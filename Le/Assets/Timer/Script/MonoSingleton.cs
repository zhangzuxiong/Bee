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
        private bool _isInited;

        /// <summary>
        /// 锁
        /// </summary>
        private static int _lock;

        /// <summary>
        /// 单例
        /// </summary>
        private static T _instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        // 以原子操作的形式，将 32 位有符号整数设置为指定的值并返回原始值。用于代替线程锁
                        if (0 == Interlocked.Exchange(ref _lock, 1))
                        {
                            try
                            {
                                if (_instance == null)
                                {
                                    _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                                    _instance.DoInit();
                                }
                            }
                            finally
                            {
                                Interlocked.Exchange(ref _lock, 0);
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                DoInit();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void DoInit()
        {
            if (!_isInited)
            {
                _isInited = true;
                if (IsDontDestroy)
                {
                    DontDestroyOnLoad(gameObject);
                }
                OnInit();
            }
        }

        public virtual void Dispose()
        {
            if (this == _instance)
            {
                _instance = null;
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (this == _instance)
            {
                _instance = null;
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
