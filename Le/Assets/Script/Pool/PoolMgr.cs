using System;

namespace Bee
{
    public static class PoolMgr
    {
        public static Pool<T> CreatePool<T>(string flag,Func<T> createObjAction, Action<T> resetObjAction = null,Action<T> destroyObjAction = null) where T :class
        {
            return new Pool<T>(flag, createObjAction, resetObjAction, destroyObjAction);
        }
    }
}