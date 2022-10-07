using UnityEngine;
using Unity.Collections;

namespace Core.GlobalVariables
{
    [CreateAssetMenu]
    public class BoolVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public bool DefaultValue;

        [SerializeField]
        private bool _value;

        public bool Value
        {
            get { return _value; }
            set
            {
                this._value = value;
                Raise(value);
            }
        }

        public delegate void ValueChange(bool newValue);
        public event ValueChange OnValueChanged;

        public void SetDefaultValue(bool value)
        {
            DefaultValue = value;
        }

        public void SetDefaultValue(BoolVariable value)
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

        public void Raise(bool newBool)
        {
            _value = newBool;
            if (OnValueChanged != null)
            {
                OnValueChanged.Invoke(newBool);
            }
        }

        private void OnEnable()
        {
            _value = DefaultValue;
        }
    }
}