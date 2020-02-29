#if !CORE_LIB
using System;
using UnityEngine;

namespace mulova.switcher
{
    internal class EnableScope : IDisposable
    {
        private bool enabled;
        public EnableScope(bool e)
        {
            enabled = GUI.enabled;
            GUI.enabled = e;
        }

        public void Dispose()
        {
            GUI.enabled = enabled;
        }
    }
}

#endif