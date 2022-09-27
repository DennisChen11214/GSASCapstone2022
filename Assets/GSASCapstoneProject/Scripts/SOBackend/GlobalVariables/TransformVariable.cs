using UnityEngine;
using Unity.Collections;

namespace Core.GlobalVariables
{
    [CreateAssetMenu]
    public class TransformVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public Transform DefaultValue;

        [SerializeField]
        private Transform _value;

        public Transform Value
        {
            get { return _value; }
            set { this._value = value; }
        }

        public delegate void ValueChange();
        public event ValueChange OnValueChanged;

        public void SetDefaultValue(Transform value)
        {
            DefaultValue = value;
        }

        public void SetDefaultValue(TransformVariable value)
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

        public void Raise(Transform newString)
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