using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;


[System.Serializable]
public class GraphContentBar : IGraphContent
{
    [SerializeField]
    string _label;

    [SerializeField]
    float[] _values;

    [SerializeField]
    Color _color = Color.red;

    [SerializeField]
    GraphEventOld _clickEvent;

    Rect[] _rects;

    public string Label { get { return _label; } set { _label = value; } }
    public float[] Values { get { return _values; } set { _values = value; } }
    public Color Color { get { return _color; } set { _color = value; } }
    public GraphEventOld ClickEvent { get { return _clickEvent; } }
    public int Count { get { return _values.Length; } }
    public Rect[] Rects { get { return _rects; } set { _rects = value; } }

    public float this[int i]
    {
        get { return _values[i]; }
        set { _values[i] = value; }
    }
}