using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using TMPro;
using System.Collections.Generic;

public class Graph : Graphic
{
    [SerializeField]
    int _segment = 10;

    [SerializeField]
    float _max, _min;

    [SerializeField]
    Color _segmentColor = new Color(0, 0, 0, 0.5f);

    [SerializeField]
    int _segmentSize = 1;

    [SerializeField]
    GraphBar _graphBar;

    [SerializeField]
    GraphLine _graphLine;
    
    GraphContext _context = new GraphContext();
    RectTransform _rectTransform;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);

        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        IGraph graph = _graphBar;
        var rect = rectTransform.rect;
        var segmentY = rect.size.y / _segment;
        for (int i = 1; i <= _segment; ++i)
        {
            DrawRect(vh, new Rect(rect.xMin, rect.yMin + i * segmentY - _segmentSize, rect.size.x, _segmentSize), _segmentColor);
        }

        // 中心
        //for (int i = 0; i < max; ++i)
        //{
        //    var x = rect.xMin + _segmentUnit.x * i + _segmentUnit.x / 2;
        //    DrawRect(vh, new Rect(x - _segmentSize / 2f, rect.yMin, 1f, rect.height), _segmentColor);
        //}

        if(!graph.Contents.Any())
        {
            return;
        }

        // グリッド
        var max = graph.Contents.Max(c => c.Count);
        for (int i = 1; i < max; ++i)
        {
            var x = rect.xMin + _context.SegmentUnit.x * i;
            DrawRect(vh, new Rect(x - _segmentSize / 2f, rect.yMin, _segmentSize, rect.height), _segmentColor);
        }

        _context.SegmentUnit = new Vector2(rect.size.x / max, segmentY);
        _context.Rect = _rectTransform.rect;
        _context.MinValue = _min;
        _context.MaxValue = _max;

        _graphBar.Draw(_context, vh);
        _graphLine.Draw(_context, vh);
    }


    public static void DrawRect(VertexHelper vh, Rect rect, Color color = default(Color))
    {
        vh.AddUIVertexQuad(new UIVertex[] {
            new UIVertex(){  position = new Vector3(rect.xMin, rect.yMin), color = color },
            new UIVertex(){  position = new Vector3(rect.xMin, rect.yMax), color = color },
            new UIVertex(){  position = new Vector3(rect.xMax, rect.yMax), color = color },
            new UIVertex(){  position = new Vector3(rect.xMax, rect.yMin), color = color },
        });
    }
}