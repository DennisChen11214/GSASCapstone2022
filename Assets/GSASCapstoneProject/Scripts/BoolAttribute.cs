///
/// Created by Dennis Chen
/// Reference: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
///

using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class BoolAttribute : PropertyAttribute
{
    public string BoolVarName;
    public bool ComparedBoolValue;

    /// <summary>
    /// Only draws the field only if a condition is met.
    /// </summary>
    public BoolAttribute(string name, bool boolValue) 
    {
        BoolVarName = name;
        ComparedBoolValue = boolValue;
    } 
}