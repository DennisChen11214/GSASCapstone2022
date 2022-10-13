///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

using UnityEngine;
using Unity.Collections;

namespace Core.GlobalVariables
{
    [CreateAssetMenu]
    public class StringVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public string DefaultValue;

        [SerializeField]
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                this._value = value;
                Raise(value);
            }
        }

        public delegate void ValueChange(string newValue);
        public event ValueChange OnValueChanged;

        public void SetDefaultValue(string value)
        {
            DefaultValue = value;
        }

        public void SetDefaultValue(StringVariable value)
        {
            DefaultValue = value.DefaultValue;
        }

        public void Subscribe(ValueChange valueChange)
        {
            OnValueChanged += valueChange;
        }

        public void Unsubscribe(ValueChange valueChange)
        {
            OnValueChanged -= valueChange;
        }

        public void Raise(string newString)
        {
            _value = newString;
            if (OnValueChanged != null)
            {
                OnValueChanged.Invoke(newString);
            }
        }

        private void OnEnable()
        {
            _value = DefaultValue;
        }
    }
}