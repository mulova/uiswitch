//----------------------------------------------
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.Ex;

namespace mulova.ui
{
    [Serializable]
    public class UISwitchPreset : ICloneable
    {
        public string presetName;
        public string[] keys = new string[0];

        public object Clone()
        {
            UISwitchPreset p = new UISwitchPreset();
            p.keys = keys.ShallowClone();
            return p;
        }
    }
}