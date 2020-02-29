//----------------------------------------------
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
#if CORE_LIB
using System.Ex;
#endif

namespace mulova.switcher
{
    [Serializable]
    public class SwitchPreset : ICloneable
    {
        public string presetName;
        public string[] keys = new string[0];

        public object Clone()
        {
            SwitchPreset p = new SwitchPreset();
            p.keys = keys.ShallowClone();
            return p;
        }
    }
}