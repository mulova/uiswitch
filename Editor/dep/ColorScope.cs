#if STANDALONE
using System;
using UnityEngine;
using Color = UnityEngine.Color;

namespace mulova.ui
{
    internal class ColorScope : IDisposable
    {
        private Color old;
        public ColorScope(Color c, bool apply = true)
        {
            old = GUI.color;
            if (apply)
            {
                GUI.color = c;
            }
        }

        public void Dispose()
        {
            GUI.color = old;
        }
    }
}
#endif