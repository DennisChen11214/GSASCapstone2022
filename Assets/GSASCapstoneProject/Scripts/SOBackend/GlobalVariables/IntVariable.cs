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

        [ReadOnlyAttribute, SerializeField]
        private int value;

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public delegate void ValueChange();
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

        public void Invoke(int amount)
        {
            value = amount;
            OnValueChanged.Invoke();
        }

        private void OnEnable()
        {
            value = DefaultValue;
        }
    }
}