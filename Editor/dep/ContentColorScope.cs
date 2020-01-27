#if STANDALONE
using System;
using UnityEngine;
using Color = UnityEngine.Color;

public class ContentColorScope : IDisposable
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
#endif