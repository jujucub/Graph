using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;

public class GraphBar : GraphOld
{
    [SerializeField]
    float _widthRatio = 0.8f;

    [SerializeField]
    List<GraphContentBar> _contents = new List<GraphContentBar>();

    [SerializeField]
    GraphEventOld _clickEvent;

    Rect[] _segmentRects;
    RectTransform _rectTransform;

    public override IEnumerable<IGraphContent> Contents
    {
        get { return _contents; }
    }

    public void AddContent(GraphContentBar content)
    {
        _contents.Add(content);
        Renderer.SetAllDirty();
    }

    public void RemoveContent(GraphContentBar content)
    {
        _contents.Remove(content);
        Renderer.SetAllDirty();
    }

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

        foreach (var content in _contents)
        {
            content.Rects = new Rect[content.Values.Length];
        }

        for (int xIndex = 0; xIndex < maxX; ++xIndex)
        {
            var contentSegmentUnitX = segmentUnit.x / _contents.Count;
            var width = contentSegmentUnitX * widthRatio;
            var segmentUnitXMin = rect.xMin + segmentUnit.x * xIndex;

            _segmentRects[xIndex] = Rect.MinMaxRect(segmentUnitXMin, rect.yMin, segmentUnitXMin + segmentUnit.x, rect.yMax);

            for (int contentsIndex = 0; contentsIndex < _contents.Count; ++contentsIndex)
            {
                var content = _contents[contentsIndex] as GraphContentBar;
                if (xIndex >= content.Values.Length)
                {
                    continue;
                }
                var x = segmentUnitXMin + contentSegmentUnitX * contentsIndex + contentSegmentUnitX / 2f;
                var value = content.Values[xIndex];
                var yUnit = rect.size.y / (maxValue - minValue);
                var yGround = rect.yMin + yUnit * -minValue;
                var yMax = Mathf.Clamp(rect.yMin + yUnit * (value - minValue), rect.yMin, rect.yMax);
                var yMin = Mathf.Clamp(yGround, rect.yMin, rect.yMax);
                content.Rects[xIndex] = Rect.MinMaxRect(x - width / 2f, yMin, x + width / 2, yMax);
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
                if (i >= content.Values.Length) { continue; }

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