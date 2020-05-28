using UnityEngine;
using UnityEditor;

namespace Graph
{
    [System.Serializable]
    public class GraphValue
    {
        [SerializeField]
        GraphPropertyFloat _value = new GraphPropertyFloat(0f);
        [SerializeField]
        GraphPropertyColor _color = new GraphPropertyColor(Color.red);

        public float Value { get { return _value.Value; } set { _value.Value = value; } }
        public Color Color { get { return _color.Value; } set { _color.Value = value; } }
    }
}