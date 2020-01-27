#if STANDALONE
using System;
using UnityEngine;

public class EnableScope : IDisposable
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
#endif