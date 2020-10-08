#if !CORE_LIB
using System;
using UnityEngine;

namespace mulova.switcher
{
    public class EnableScope : IDisposable
    {
        private bool enabled;
        public EnableScope(bool e, bool overwrite = true)
        {
            enabled = GUI.enabled;
            if (overwrite)
            {
                GUI.enabled = e;
            }
            else
            {
                GUI.enabled &= e;
            }
        }

        public void Dispose()
        {
            GUI.enabled = enabled;
        }
    }
}

#endif