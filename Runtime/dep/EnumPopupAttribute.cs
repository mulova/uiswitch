#if STANDALONE
using UnityEngine;

namespace mulova.ui
{
    public class EnumPopupAttribute : PropertyAttribute
    {
        public readonly string enumTypeVar;
        
        public EnumPopupAttribute(string enumVar)
        {
            this.enumTypeVar = enumVar;
        }
    }
}

#endif