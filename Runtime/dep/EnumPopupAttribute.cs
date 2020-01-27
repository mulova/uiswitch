using UnityEngine;

public class EnumPopupAttribute : PropertyAttribute
{
    public readonly string enumTypeVar;
    
    public EnumPopupAttribute(string enumVar)
    {
        this.enumTypeVar = enumVar;
    }
}
