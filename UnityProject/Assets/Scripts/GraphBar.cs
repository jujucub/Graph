using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GraphBar : Graph
{
    [SerializeField]
    float _widthRatio = 0.8f;

    [SerializeField]
    GraphContentBar[] _contents;

    [SerializeField]
    GraphEvent _clickEvent;

    Rect[] _segmentRects;
    RectTransform _rectTransform;

    public override IGraphContent[] Contents { get { return _contents; } }

    public override void OnDraw(VertexHelper vh)
    {
        if (!_contents.Any())
        {
            return;
        }

        var rect = Renderer.RectTransform.rect;
        var segmentUnit = Renderer.SegmentUnit;
        var widthRatio = _widthRatio;
        var maxX = _contents.Max(c => c.Count);
        var maxValue = Renderer.MaxValue;
        var minValue = Renderer.MinValue;

        _segmentRects = new Rect[maxX];

        foreach(var content in _contents)
        {
            content.Rects = new Rect[content.Values.Length];
        }

        for (int xIndex = 0; xIndex < maxX; ++xIndex)
        {
            var contentSegmentUnitX = segmentUnit.x / _contents.Length;
            var width = contentSegmentUnitX * widthRatio;
            var segmentUnitXMin = rect.xMin + segmentUnit.x * xIndex;

            _segmentRects[xIndex] = Rect.MinMaxRect(segmentUnitXMin, rect.yMin, segmentUnitXMin + segmentUnit.x, rect.yMax);

            for (int contentsIndex = 0; contentsIndex < _contents.Length; ++contentsIndex)
            {
                var content = _contents[contentsIndex] as GraphContentBar;
                if (xIndex >= content.Values.Length)
                {
                    continue;
                }

                var x = segmentUnitXMin + contentSegmentUnitX * contentsIndex + contentSegmentUnitX / 2;
                var value = content.Values[xIndex];
                var yGround = rect.size.y / (maxValue - minValue) * -minValue;
                var yMax = rect.size.y / (maxValue - minValue) * maxValue;
                var yMin = rect.size.y / (maxValue - minValue) * minValue;
                var height = (value - minValue) / (maxValue - minValue) * rect.size.y - yGround;
                height = Mathf.Clamp(height, yMin, yMax);
                content.Rects[xIndex] = new Rect(x - width / 2, rect.yMin + yGround, width, height);
                GraphRenderer.DrawRect(vh, content.Rects[xIndex], content.Color);
            }
        }
    }

    public override void OnPointerDown(Vector2 position)
    {
        for (int i = 0; i < _segmentRects.Length; ++i)
        {
            if (_segmentRects[i].Contains(position))
            {
            }
        }
    }

    public override void OnPointerEnter(Vector2 position)
    {
        for (int i = 0; i < _segmentRects.Length; ++i)
        {
            if (_segmentRects[i].Contains(position))
            {
            }
        }
    }

    public override void OnPointerExit(Vector2 position)
    {
        for (int i = 0; i < _segmentRects.Length; ++i)
        {
            if (_segmentRects[i].Contains(position))
            {
            }
        }
    }

    public override void OnPointerUp(Vector2 position)
    {
        for (int i = 0; i < _segmentRects.Length; ++i)
        {
            if (_segmentRects[i].Contains(position))
            {
            }
        }
    }

    public override void OnPointerClick(Vector2 position)
    {
        for (int i = 0; i < _segmentRects.Length; ++i)
        {
            // セグメント自体の判定
            if (_segmentRects[i].Contains(position))
            {
                _clickEvent.Invoke(i);
                Debug.Log($"Click Segment {i}");
            }

            // バーの判定
            foreach (var content in _contents)
            {
                if(i >= content.Values.Length) { continue; }

                if (content.Rects[i].Contains(position))
                {
                    Debug.Log($"Click Bar {i}");
                    content.ClickEvent.Invoke(i);
                }
            }
        }
    }

    Vector2 ScreenPositionToLocalPosition(Vector2 screenPosition)
    {
        return _rectTransform.InverseTransformPoint(screenPosition);
    }
}