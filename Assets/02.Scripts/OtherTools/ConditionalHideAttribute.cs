#if UNITY_EDITOR
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public readonly string ConditionalSourceField;
    public string ConditionalSourceField2 = "";
    public bool HideInInspector = false;
    public bool Inverse = false;

    // Use this for initialization
    public ConditionalHideAttribute(string conditionalSourceField)
    {
        ConditionalSourceField = conditionalSourceField;
        HideInInspector = false;
    }
    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
    {
        ConditionalSourceField = conditionalSourceField;
        HideInInspector = hideInInspector;
    }
}
#endif