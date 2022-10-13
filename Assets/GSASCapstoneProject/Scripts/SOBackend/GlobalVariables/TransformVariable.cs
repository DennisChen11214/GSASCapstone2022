///
/// Created by Dennis Chen
/// Reference: Ryan's code talked about in https://www.youtube.com/watch?v=raQ3iHhE_Kk
///

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
            set
            {
                this._value = value;
                Raise(value);
            }
        }

        public delegate void ValueChange(Transform newValue);
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

        public void Unsubscribe(ValueChange valueChange)
        {
            OnValueChanged -= valueChange;
        }

        public void Raise(Transform newTransform)
        {
            _value = newTransform;
            if (OnValueChanged != null)
            {
                OnValueChanged.Invoke(newTransform);
            }
        }

        private void OnEnable()
        {
            _value = DefaultValue;
        }
    }
}