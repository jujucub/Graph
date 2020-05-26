﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class GraphEvent : UnityEvent<int> {}

[RequireComponent(typeof(GraphRenderer))]
public abstract class Graph : MonoBehaviour
{
    GraphRenderer _renderer;

    public GraphRenderer Renderer
    {
        get
        {
            if(_renderer == null)
            {
                _renderer = GetComponent<GraphRenderer>();
            }
            return _renderer;
        }
    }

    public abstract IGraphContent[] Contents { get; }

    public abstract void OnDraw(VertexHelper vh);
    public virtual void OnPointerDown(Vector2 position) { }
    public virtual void OnPointerEnter(Vector2 position) { }
    public virtual void OnPointerExit(Vector2 position) { }
    public virtual void OnPointerUp(Vector2 position) { }
    public virtual void OnPointerClick(Vector2 position) { }

#if UNITY_EDITOR
    void OnValidate()
    {
        Renderer.SetVerticesDirty();
    }
#endif

}