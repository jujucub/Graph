using UnityEngine;
using UnityEditor;

namespace Graph
{
    [System.Serializable]
    public class GraphContent
    {
        [SerializeField]
        GraphValue[] _values;
        public GraphValue[] Values { get { return _values; } set { _values = value; } }
    }
}