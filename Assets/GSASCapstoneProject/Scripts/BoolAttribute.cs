///
/// Created by Dennis Chen
/// Reference: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
///

using UnityEngine;
using System;

//An attribute where given the name of a boolean variable and the optional parent object it belongs to, if the value
//of that boolean is equal to a given boolean, then display that field in the inspector. Otherwise, don't display it.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class BoolAttribute : PropertyAttribute
{
    public string VarParent;
    public string BoolVarName;
    public bool ComparedBoolValue;

    /// <summary>
    /// Only draws the field only if a condition is met.
    /// </summary>
    public BoolAttribute(string name, bool boolValue, string varParent = "") 
    {
        BoolVarName = name;
        ComparedBoolValue = boolValue;
        VarParent = varParent;
    } 
}