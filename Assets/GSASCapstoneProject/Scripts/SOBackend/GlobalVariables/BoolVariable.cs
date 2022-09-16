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
        private bool value;

        public bool Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public delegate void ValueChange();
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

        public void UnSubscribe(ValueChange valueChange)
        {
            OnValueChanged -= valueChange;
        }

        public void Raise(bool boolean)
        {
            value = boolean;
            OnValueChanged.Invoke();
        }

        private void OnEnable()
        {
            value = DefaultValue;
        }
    }
}