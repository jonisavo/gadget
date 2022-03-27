using System;
using System.Collections.Generic;

using Object = UnityEngine.Object;

namespace Gadget.Editor.Internal.Scope
{
    internal readonly struct ObjectScope : IDisposable
    {
        private static readonly HashSet<int> ObjectScopeSet =
            new HashSet<int>();

        private readonly int _instanceID;

        public ObjectScope(Object obj)
        {
            _instanceID = obj.GetInstanceID();
            ObjectScopeSet.Add(_instanceID);
        }

        public void Dispose()
        {
            ObjectScopeSet.Remove(_instanceID);
        }

        public static bool Contains(Object obj)
        {
            if (obj == null)
                return false;
            var instanceID = obj.GetInstanceID();
            return ObjectScopeSet.Contains(instanceID);
        }
    }
}