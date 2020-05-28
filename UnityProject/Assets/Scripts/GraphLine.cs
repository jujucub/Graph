using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class GraphLine : GraphOld
{
    [SerializeField]
    float _width = 5f;

    [SerializeField]
    List<GraphContentLine> _contents;

    public override IEnumerable<IGraphContent> Contents { get { return _contents; } }

    public void AddContent(GraphContentLine content)
    {
        _contents.Add(content);
    }

    public void RemoveContent(GraphContentLine content)
    {
        _contents.Remove(content);
    }

    public override void OnDraw(VertexHelper vh)
    {
        if (!Contents.Any())
        {
            return;
        }

        var rect = Renderer.rectTransform.rect;
        var segmentUnit = Renderer.SegmentUnit;
        var maxX = _contents.Max(c => c.Count);
        var maxValue = Renderer.MaxValue;
        var minValue = Renderer.MinValue;

        for (int xIndex = 1; xIndex < maxX; ++xIndex)
        {
            var contentSegmentX = rect.xMin + xIndex * segmentUnit.x + segmentUnit.x / 2f;
            var beforeContentSegmentX = rect.xMin + (xIndex - 1) * segmentUnit.x + segmentUnit.x / 2f;

            for (int contentsIndex = 0; contentsIndex < _contents.Count; ++contentsIndex)
            {
                var content = _contents[contentsIndex] as GraphContentLine;
                if (xIndex >= content.Values.Length)
                {
                    continue;
                }
                var value = content.Values[xIndex];
                var beforeValue = content.Values[xIndex - 1];
                var height = (value - minValue) / (maxValue - minValue) * rect.size.y;
                var beforeHeight = (beforeValue - minValue) / (maxValue - minValue) * rect.size.y;

                var vec1 = new Vector2(beforeContentSegmentX, beforeHeight);
                var vec2 = new Vector2(contentSegmentX, height);

                var y0 = beforeHeight + _width / 2f;
                var y1 = beforeHeight - _width / 2f;
                var y2 = height - _width / 2f;
                var y3 = height + _width / 2f;

                var color = content.Color;

                var subdivition = 4;
                var dir1 = vec2 - vec1;
                dir1.Normalize();
                var angle = Mathf.Atan2(dir1.y, dir1.x);

                Debug.Log("angle " + angle);
                var angleSegmentUnit = angle / subdivition;
                var startAngle = Mathf.PI / 2 - angle / 2f;
                var halfPI = Mathf.PI / 2f;

                var vertCount = vh.currentVertCount;
                var indexCount = vh.currentIndexCount;

                var rightUnit = new Vector2(dir1.y, -dir1.x);
                var leftUnit = new Vector2(-dir1.y,  dir1.x);
                var right = rightUnit * _width;
                var left = leftUnit * _width;

                var x0 = beforeContentSegmentX + left.x;
                var x1 = beforeContentSegmentX + right.x;
                var x2 = contentSegmentX + right.x;
                var x3 = contentSegmentX + left.x;

                if(xIndex + 1 < content.Values.Length)
                {
                    var nextValue = content.Values[xIndex + 1];
                    var nextContentSegmentX = rect.xMin + (xIndex + 1) * segmentUnit.x + segmentUnit.x / 2f;
                    var vec3 = new Vector2(nextContentSegmentX, height);
                    var dir2 = vec3 - vec2;
                    dir2.Normalize();
                    var cornerAngle = Vector2.Angle(dir1, dir2) * Mathf.Deg2Rad;

                    //var cornerStartAngle = angle - cornerAngle / 2f;
                    //for(int i = 0;i < 4; ++i)
                    //{
                    //    vh.AddVert(new UIVertex() { position = new Vector3(x2, y2 + right.y), color = color });
                    //    vh.AddVert(new UIVertex() { position = new Vector3(x3, y3 + left.y), color = color });
                    //    vh.AddTriangle(vertCount, vertCount + 1, vertCount + 2);
                    //    vh.AddTriangle(vertCount, vertCount + 2, vertCount + 3);
                    //}
                }

                vh.AddVert(new UIVertex() { position = new Vector3(x0, y0 + left.y), color = color });
                vh.AddVert(new UIVertex() { position = new Vector3(x1, y1 + right.y), color = color });
                vh.AddVert(new UIVertex() { position = new Vector3(x2, y2 + right.y), color = color });
                vh.AddVert(new UIVertex() { position = new Vector3(x3, y3 + left.y), color = color });
                vh.AddTriangle(vertCount, vertCount + 1, vertCount + 2);
                vh.AddTriangle(vertCount, vertCount + 2, vertCount + 3);

                //vh.AddUIVertexQuad(new UIVertex[] {
                //    new UIVertex() { position = new Vector3(beforeContentSegmentX + left.x, y0 + left.y), color = color },
                //    new UIVertex() { position = new Vector3(beforeContentSegmentX + right.x, y1 + right.y), color = color },
                //    new UIVertex() { position = new Vector3(contentSegmentX + right.x, y2 + right.y), color = color },
                //    new UIVertex() { position = new Vector3(contentSegmentX + left.x, y3 + left.y), color = color },
                //});

                //for (int i = 0; i < subdivition; ++i)
                //{
                //    var angleUnit = startAngle + i * angleSegmentUnit;
                //    vh.AddVert(new UIVertex() { position = new Vector3(beforeContentSegmentX, y0), color = color });
                //    vh.AddVert(new UIVertex() { position = new Vector3(beforeContentSegmentX, y0), color = color });
                //    vh.AddVert(new UIVertex() { position = new Vector3(beforeContentSegmentX, y0), color = color });
                //    vh.AddVert(new UIVertex() { position = new Vector3(beforeContentSegmentX, y0), color = color });
                //}


                //vh.AddUIVertexQuad(new UIVertex[] {
                //    new UIVertex(){  position = new Vector3(beforeContentSegmentX, y0), color = color },
                //    new UIVertex(){  position = new Vector3(beforeContentSegmentX, y1), color = color },
                //    new UIVertex(){  position = new Vector3(contentSegmentX, y2), color = color },
                //    new UIVertex(){  position = new Vector3(contentSegmentX, y3), color = color },
                //});
            }
        }
    }
}