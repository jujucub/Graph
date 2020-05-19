using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class GraphBar : IGraph
{
    [SerializeField]
    float _widthRatio = 0.8f;

    [SerializeField]
    GraphContentBar[] _contents;

    public IGraphContent[] Contents { get { return _contents; } }

    public void Draw(GraphContext graphContext, VertexHelper vh)
    {
        if(!_contents.Any())
        {
            return;
        }

        var rect = graphContext.Rect;
        var segmentUnit = graphContext.SegmentUnit;
        var widthRatio = _widthRatio;
        var maxX = _contents.Max(c => c.Count);
        var maxValue = graphContext.MaxValue;
        var minValue = graphContext.MinValue;

        for (int xIndex = 0; xIndex < maxX; ++xIndex)
        {
            var contentSegmentUnitX = segmentUnit.x / _contents.Length;
            var width = contentSegmentUnitX * widthRatio;
            var segmentUnitXMin = rect.xMin + segmentUnit.x * xIndex;
            for (int contentsIndex = 0; contentsIndex < _contents.Length; ++contentsIndex)
            {
                var content = _contents[contentsIndex] as GraphContentBar;
                if (xIndex >= content.Values.Length)
                {
                    continue;
                }

                var x = segmentUnitXMin + contentSegmentUnitX * contentsIndex + contentSegmentUnitX / 2;
                var value = content.Values[xIndex];
                var height = (value - minValue) / (maxValue - minValue) * rect.size.y;
                Graph.DrawRect(vh, new Rect(x - width / 2, rect.yMin, width, height), content.Color);
            }
        }
    }
}