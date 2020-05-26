using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public interface IGraph
{
    IGraphContent[] Contents { get; }
    void Draw(GraphContext graphContext, VertexHelper vh);
    void OnPointerEnter(Vector2 position);
    void OnPointerExit(Vector2 position);
    void OnPointerUp(Vector2 position);
    void OnPointerDown(Vector2 position);
}