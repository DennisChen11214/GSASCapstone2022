///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using System;

namespace Core.GlobalVariables
{
    [Serializable]
    public class IntReference
    {
        public bool UseConstant = true;
        public int ConstantValue;
        public IntVariable Variable;

        public IntReference()
        { }

        public IntReference(int value)
        {
            UseConstant = true;
            ConstantValue = value;
        }

        public int Value
        {
            get { return UseConstant ? ConstantValue : Variable.DefaultValue; }
        }

        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
    }
}