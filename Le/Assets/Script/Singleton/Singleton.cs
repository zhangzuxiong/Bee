using System.Threading;

namespace Bee
{
    public class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static T _instance;
        /// <summary>
        /// 锁
        /// </summary>
        private static int _lock;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 以原子操作的形式，将 32 位有符号整数设置为指定的值并返回原始值。用于代替线程锁
                    if (0 == Interlocked.Exchange(ref _lock, 1))
                    {
                        try
                        {
                            if (_instance == null)
                            {
                                //创建单例的实例
                                _instance = new T();
                                _instance.Init();
                            }
                        }
                        finally
                        {
                            Interlocked.Exchange(ref _lock, 0);
                        }
                    }
                }
                return _instance;
            }
        }

        public virtual void Dispose()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void Init()
        {
            OnInit();
        }

        public virtual void OnInit()
        {

        }
    }
}
