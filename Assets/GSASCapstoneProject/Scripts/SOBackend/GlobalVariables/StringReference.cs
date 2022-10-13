///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using System;

namespace Core.GlobalVariables
{
    [Serializable]
    public class StringReference
    {
        public bool UseConstant = true;
        public string ConstantValue;
        public StringVariable Variable;

        public StringReference()
        { }

        public StringReference(string value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public string Value
        {
            get { return UseConstant ? ConstantValue : Variable.DefaultValue; }
        }

        public static implicit operator string(StringReference reference)
        {
            return reference.Value;
        }
    }
}