///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using UnityEngine;
using Unity.Collections;

namespace Core.GlobalVariables
{
    [CreateAssetMenu]
    public class IntVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public int DefaultValue;

        [SerializeField]
        private int _value;

        public int Value
        {
            get { return _value; }
            set 
            { 
                this._value = value;
                Raise(value);
            }
        }

        public delegate void ValueChange(int newValue);
        public event ValueChange OnValueChanged;

        public void SetDefaultValue(int value)
        {
            DefaultValue = value;
        }

        public void SetDefaultValue(IntVariable value)
        {
            DefaultValue = value.DefaultValue;
        }

        public void AddToDefault(int amount)
        {
            DefaultValue += amount;
        }

        public void AddToDefault(IntVariable amount)
        {
            DefaultValue += amount.DefaultValue;
        }

        public void Subscribe(ValueChange valueChange)
        {
            OnValueChanged += valueChange;
        }

        public void Unsubscribe(ValueChange valueChange)
        {
            OnValueChanged -= valueChange;
        }

        public void Raise(int amount)
        {
            _value = amount;
            if(OnValueChanged != null)
            {
                OnValueChanged.Invoke(amount);
            }
        }

        private void OnEnable()
        {
            _value = DefaultValue;
        }
    }
}