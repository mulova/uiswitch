#if !CORE_LIB
using System;
using UnityEngine;
using Color = UnityEngine.Color;

namespace mulova.ui
{
    internal class ContentColorScope : IDisposable
    {
        private Color old;
        public ContentColorScope(Color c, bool apply = true)
        {
            old = GUI.contentColor;
            if (apply)
            {
                GUI.contentColor = c;
            }
        }

        public void Dispose()
        {
            GUI.contentColor = old;
        }
    }
}
#endif