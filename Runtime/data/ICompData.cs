using System;
using UnityEngine;

namespace mulova.ui
{
    public interface ICompData
    {
        Type type { get; }
        bool active { get; }
        Component target { get; set; }
        void ApplyTo(Component c);
        void Collect(Component c);
    }
}

