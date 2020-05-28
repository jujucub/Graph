using UnityEngine;
using UnityEditor;

namespace Graph
{
    public interface IGraphProperty
    {
        System.Action OnValueChange { get; set; }
    }

    [System.Serializable]
    public class GraphProperty<T> : IGraphProperty
    {
        [SerializeField]
        T _value;

        public System.Action OnValueChange { get; set; }

        public GraphProperty()
        {
            _value = default(T);
        }

        public GraphProperty(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
            set {
                _value = value;
                OnValueChange.SafeInvoke();
            }
        }
    }

    [System.Serializable]
    public class GraphPropertyFloat : GraphProperty<float> { public GraphPropertyFloat(float value) : base(value) { } }
    [System.Serializable]
    public class GraphPropertyColor : GraphProperty<Color> { public GraphPropertyColor(Color value) : base(value) { } }
    [System.Serializable]
    public class GraphPropertyInt : GraphProperty<int> { public GraphPropertyInt(int value) : base(value) { } }

#if UNITY_EDITOR



#endif
}
