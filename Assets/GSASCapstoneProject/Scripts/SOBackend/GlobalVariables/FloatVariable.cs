using UnityEngine;
using Unity.Collections;

namespace Core.GlobalVariables
{
    [CreateAssetMenu]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public float DefaultValue;

        [SerializeField]
        private float _value;

        public float Value
        {
            get { return _value;}
            set { this._value = value;}
        }

        public delegate void ValueChange();
        public event ValueChange OnValueChanged; 

        public void SetDefaultValue(float value)
        {
            DefaultValue = value;
        }

        public void SetDefaultValue(FloatVariable value)
        {
            DefaultValue = value.DefaultValue;
        }

        public void AddToDefault(float amount)
        {
            DefaultValue += amount;
        }

        public void AddToDefault(FloatVariable amount)
        {
            DefaultValue += amount.DefaultValue;
        }

        public void Subscribe(ValueChange valueChange)
        {
            OnValueChanged += valueChange;
        }

        public void UnSubscribe(ValueChange valueChange)
        {
            OnValueChanged -= valueChange;
        }

        public void Raise(float amount)
        {
            _value = amount;
            OnValueChanged.Invoke();
        }

        private void OnEnable()
        {
            _value = DefaultValue;
        }
    }
}