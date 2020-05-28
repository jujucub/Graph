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
    List<GraphOld> _graphs;

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

    bool _isUpdateNumber = false;
    int _rowCount;
    Vector2 _segmentUnit;
    RectTransform _rectTransform;

    protected override void Start()
    {
        SetVerticesDirty();
        base.Start();
    }

    public T CreateGraph<T>() where T : GraphOld
    {
        var graph = gameObject.AddComponent<T>();
        _graphs.Add(graph);
        return graph;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);

        Debug.Log("OnPopulateMesh");

        var rect = rectTransform.rect;
        for (int i = 0; i <= _segment; ++i)
        {
            DrawRect(vh, new Rect(rect.xMin, rect.yMin + i * _segmentUnit.y - _segmentSize, rect.size.x, _segmentSize), _segmentColor);
        }

        if (!_graphs.Any())
        {
            return;
        }

        // グリッド
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

        // グラフの描画
        foreach (var graph in _graphs)
        {
            graph.OnDraw(vh);
        }

        _isUpdateNumber = true;
    }

    public void UpdateParameter()
    {
        // X軸の最大のグラフを取得.
        _segmentUnit.x = 0f;
        _segmentUnit.y = RectTransform.rect.size.y / _segment;

        GraphOld maxGraph = _graphs.OrderByDescending(g => g.Contents.Count()).FirstOrDefault();
        if (maxGraph == null || !maxGraph.Contents.Any())
        {
            _rowCount = 0;
            return;
        }

        _rowCount = maxGraph.Contents.Max(c => c.Count);
        _segmentUnit.x = RectTransform.rect.size.x / _rowCount;
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
        if(_isUpdateNumber)
        {
            Debug.Log("UpdateNumber");

            if (_numbers == null || transform.childCount != _segment + 1)
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

            if (_labels == null || _labels.Length != _rowCount)
            {

            }

            _isUpdateNumber = false;
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateParameter();
    }
#endif
}