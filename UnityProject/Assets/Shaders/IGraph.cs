using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public interface IGraph
{
    IGraphContent[] Contents { get; }
    void Draw(GraphContext graphContext, VertexHelper vh);
}