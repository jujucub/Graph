using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using TMPro;
using System.Collections.Generic;


[ExecuteInEditMode]
public class GraphRenderer : Graphic, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    int _segment = 10;

    [SerializeField]
    float _maxValue, _minValue;

    [SerializeField]
    Color _segmentColor = new Color(0, 0, 0, 0.5f);

    [SerializeField]
    int _segmentSize = 1;

    [SerializeField]
    TextMeshProUGUI _customNumber;

    [SerializeField]
    string _numberFormat = "";

    [SerializeField]
    Graph[] _graphs;

    [SerializeField]
    TextMeshProUGUI[] _numbers;

    [SerializeField]
    TextMeshProUGUI[] _labels;


    public float MinValue { get { return _minValue; } }
    public float MaxValue { get { return _maxValue; } }
    public int SegmentSize { get { return _segmentSize; } }
    public Vector2 SegmentUnit { get { return _segmentUnit; } }

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    bool _isDirty = false;
    int _rowCount;
    Vector2 _segmentUnit;
    RectTransform _rectTransform;

    protected override void Start()
    {
        SetVerticesDirty();
        base.Start();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);

        var rect = rectTransform.rect;
        var segmentY = rect.size.y / _segment;
        for (int i = 0; i <= _segment; ++i)
        {
            DrawRect(vh, new Rect(rect.xMin, rect.yMin + i * segmentY - _segmentSize, rect.size.x, _segmentSize), _segmentColor);
        }

        if (!_graphs.Any())
        {
            return;
        }

        // X軸の最大のグラフを取得.
        Graph maxGraph = _graphs.OrderByDescending(g => g.Contents.Length).FirstOrDefault();

        // グリッド
        _rowCount = maxGraph.Contents.Max(c => c.Count);
        for (int i = 0; i < _rowCount + 1; ++i)
        {
            var x = rect.xMin + _segmentUnit.x * i;
            DrawRect(vh, new Rect(x - _segmentSize / 2f, rect.yMin, _segmentSize, rect.height), _segmentColor);
        }

        // 中心
        for (int i = 0; i < _rowCount; ++i)
        {
            var x = rect.xMin + _segmentUnit.x * i + _segmentUnit.x / 2;
            DrawRect(vh, new Rect(x - _segmentSize / 2f, rect.yMin, 1f, rect.height), _segmentColor);
        }

        _segmentUnit = new Vector2(rect.size.x / _rowCount, segmentY);

        // グラフの描画.
        foreach (var graph in _graphs)
        {
            graph.OnDraw(vh);
        }
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

    public void OnPointerDown(PointerEventData eventData)
    {
        var position = ScreenPositionToLocalPosition(eventData.position);
        foreach (var graph in _graphs)
        {
            graph.OnPointerDown(position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var position = ScreenPositionToLocalPosition(eventData.position);
        foreach (var graph in _graphs)
        {
            graph.OnPointerUp(position);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var position = ScreenPositionToLocalPosition(eventData.position);
        foreach (var graph in _graphs)
        {
            graph.OnPointerEnter(position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var position = ScreenPositionToLocalPosition(eventData.position);
        foreach (var graph in _graphs)
        {
            graph.OnPointerExit(position);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var position = ScreenPositionToLocalPosition(eventData.position);
        foreach (var graph in _graphs)
        {
            graph.OnPointerClick(position);
        }
    }

    Vector2 ScreenPositionToLocalPosition(Vector2 screenPosition)
    {
        return RectTransform.InverseTransformPoint(screenPosition);
    }

    void Update()
    {
        if (_numbers == null || _numbers.Length != _segment + 1)
        {
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            _numbers = new TextMeshProUGUI[_segment + 1];
            for (int i = 0; i < _segment + 1; ++i)
            {
                if (_customNumber != null)
                {
                    _numbers[i] = Instantiate(_customNumber, transform);
                }
                else
                {
                    _numbers[i] = new GameObject($"{i}", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
                    _numbers[i].transform.SetParent(transform);
                }

                _numbers[i].rectTransform.anchorMin = new Vector2(0f, 0.5f);
                _numbers[i].rectTransform.anchorMax = new Vector2(0f, 0.5f);
                _numbers[i].rectTransform.pivot = new Vector2(1f, 0.5f);
                _numbers[i].alignment = TextAlignmentOptions.MidlineRight;
                _numbers[i].rectTransform.anchoredPosition = new Vector2(-5f, RectTransform.rect.yMin + _segmentUnit.y * i);
                _numbers[i].text = (_minValue + ((_maxValue - _minValue) / _segment) * i).ToString(_numberFormat);
                _numbers[i].color = Color.black;
            }
        }
        else
        {
            for (int i = 0; i < _segment + 1; ++i)
            {
                var number = (_minValue + ((_maxValue - _minValue) / _segment) * i).ToString();
                _numbers[i].rectTransform.anchoredPosition = new Vector2(-5f, RectTransform.rect.yMin + _segmentUnit.y * i);
                if (number != _numbers[i].text)
                {
                    _numbers[i].text = (_minValue + ((_maxValue - _minValue) / _segment) * i).ToString(_numberFormat);
                }
            }
        }

        if(_labels == null || _labels.Length != _rowCount)
        {

        }
    }
}