using System.Threading;

namespace Bee
{
    public class Singleton<T> : ISingleton where T : Singleton<T>, new()
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static T _Instance;
        /// <summary>
        /// 锁
        /// </summary>
        private static int _Lock;
        /// <summary>
        /// 单例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_Instance == null)
                {
                    // 以原子操作的形式，将 32 位有符号整数设置为指定的值并返回原始值。用于代替线程锁
                    if (0 == Interlocked.Exchange(ref _Lock, 1))
                    {
                        try
                        {
                            if (_Instance == null)
                            {
                                //创建单例的实例
                                _Instance = new T();
                                _Instance.OnInit();
                            }
                        }
                        finally
                        {
                            Interlocked.Exchange(ref _Lock, 0);
                        }
                    }
                }
                return _Instance;
            }
        }

        public virtual void Dispose()
        {
            if (_Instance == this)
            {
                _Instance = null;
            }
        }

        public virtual void OnInit()
        {

        }
    }
}
