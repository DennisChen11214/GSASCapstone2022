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
            set { this._value = value; }
        }

        public delegate void ValueChange();
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

        public void UnSubscribe(ValueChange valueChange)
        {
            OnValueChanged -= valueChange;
        }

        public void Raise(string newString)
        {
            _value = newString;
            OnValueChanged.Invoke();
        }

        private void OnEnable()
        {
            _value = DefaultValue;
        }
    }
}