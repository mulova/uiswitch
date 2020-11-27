using System;
using UnityEngine;

namespace mulova.switcher
{
    // NOTE: Equals() and GetHashCode() must be implemented
    public interface ICompData
    {
        Type type { get; }
        bool active { get; }
        Component target { get; set; }
        void ApplyTo(Component c);
        void Collect(Component c);
    }
}

