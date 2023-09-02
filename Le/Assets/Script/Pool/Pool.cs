using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bee
{
    public interface IPool<T> : IDisposable
    {
        T GetObjFromPool();
        void RecoverObjToPool(T item);
    }
    public abstract class PoolBase<T>
    {
        protected string flag;
        protected Queue<T> objPools;
        protected Func<T> createObjAction;
        protected Action<T> resetObjAction;
        protected Action<T> destroyObjAction;
    }

    public class Pool<T> : PoolBase<T>,IPool<T> where T : class
    {
        public Pool()
        {
            objPools = new Queue<T>();
        }
        public Pool(string flag,Func<T> createObjAction, Action<T> resetObjAction = null,Action<T> destroyObjAction = null):this()
        {
            this.flag = flag;
            this.createObjAction = createObjAction;
            this.resetObjAction = resetObjAction;
            this.destroyObjAction = destroyObjAction;
        }

        public void Dispose()
        {
            while (objPools.Count > 0)
            {
                destroyObjAction?.Invoke(objPools.Dequeue());
            }
        }

        public T GetObjFromPool()
        {
            T obj;
            if (!objPools.TryDequeue(out obj))
            {
                obj = createObjAction.Invoke();
            }

            return obj;
        }

        public void RecoverObjToPool(T obj)
        {
            resetObjAction?.Invoke(obj);
            objPools.Enqueue(obj);
        }
    }
}