using System;

namespace Bee
{
    public interface ISingleton : IDisposable
    {
        void Init();
    }
}