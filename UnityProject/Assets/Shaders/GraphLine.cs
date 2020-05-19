using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class GraphLine : IGraph
{
    [SerializeField]
    float _width = 5f;

    [SerializeField]
    GraphContentLine[] _contents;

    public IGraphContent[] Contents { get { return _contents; } }

    public void Draw(GraphContext graphContext, VertexHelper vh)
    {
        if(!Contents.Any())
        {
            return;
        }

        var rect = graphContext.Rect;
        var segmentUnit = graphContext.SegmentUnit;
        var maxX = _contents.Max(c => c.Count);
        var maxValue = graphContext.MaxValue;
        var minValue = graphContext.MinValue;

        for (int xIndex = 1; xIndex < maxX; ++xIndex)
        {
            var contentSegmentX = rect.xMin + xIndex * segmentUnit.x + segmentUnit.x / 2f;
            var beforeContentSegmentX = rect.xMin + (xIndex - 1) * segmentUnit.x + segmentUnit.x / 2f;

            for (int contentsIndex = 0; contentsIndex < _contents.Length; ++contentsIndex)
            {
                var content = _contents[contentsIndex] as GraphContentLine;
                if (xIndex >= content.Values.Length)
                {
                    continue;
                }
                var value = content.Values[xIndex];
                var beforeValue = content.Values[xIndex-1];
                var height = (value - minValue) / (maxValue - minValue) * rect.size.y;
                var beforeHeight = (beforeValue - minValue) / (maxValue - minValue) * rect.size.y;

                var y0 = beforeHeight + _width / 2f;
                var y1 = beforeHeight - _width / 2f;
                var y2 = height - _width / 2f;
                var y3 = height + _width / 2f;

                var color = content.Color;
                vh.AddUIVertexQuad(new UIVertex[] {
                    new UIVertex(){  position = new Vector3(beforeContentSegmentX, y0), color = color },
                    new UIVertex(){  position = new Vector3(beforeContentSegmentX, y1), color = color },
                    new UIVertex(){  position = new Vector3(contentSegmentX, y2), color = color },
                    new UIVertex(){  position = new Vector3(contentSegmentX, y3), color = color },
                });
            }
        }
    }
}