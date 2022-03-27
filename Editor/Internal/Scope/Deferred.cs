using System;

namespace Gadget.Editor.Internal.Scope
{
    internal readonly struct Deferred : IDisposable
    {
        private readonly Action _onDispose;

        public Deferred(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose?.Invoke();
        }
    }
}