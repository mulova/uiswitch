using System;
using UnityEngine;

namespace mulova.ui
{
    public interface ICompData
    {
        Type type { get; }
        Component target { get; }
        void ApplyTo(Component c);
        void Collect(Component c);
    }
}

