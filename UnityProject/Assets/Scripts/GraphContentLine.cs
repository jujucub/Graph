using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class GraphContentLine : IGraphContent
{
    [SerializeField]
    string _label;

    [SerializeField]
    float[] _values;

    [SerializeField]
    Color _color = Color.red;

    public int Count { get { return _values.Length; } }

    public float this[int i]
    {
        get { return _values[i]; }
        set { _values[i] = value; }
    }

    public string Label { get { return _label; } set { _label = value; } }
    public float[] Values { get { return _values; } set { _values = value; } }
    public Color Color { get { return _color; } set { _color = value; } }

}