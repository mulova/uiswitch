﻿#if !CORE_LIB
using System;
using UnityEngine;
using Color = UnityEngine.Color;

namespace mulova.switcher
{
    public class ColorScope : IDisposable
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