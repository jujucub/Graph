using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Graph
{
    public class GraphBarRenderer : GraphRenderer, IPointerClickHandler
    {
        [SerializeField]
        GraphPropertyInt _column = new GraphPropertyInt(10);
        [SerializeField]
        GraphPropertyInt _row = new GraphPropertyInt(10);
        [SerializeField]
        GraphPropertyInt _gridSize = new GraphPropertyInt(2);
        [SerializeField]
        GraphPropertyFloat _maxValue = new GraphPropertyFloat(10), _minValue = new GraphPropertyFloat(0);
        [SerializeField]
        GraphPropertyColor _gridColor = new GraphPropertyColor(new Color(0, 0, 0, 0.5f));
        [SerializeField]
        GraphPropertyFloat _widthRatio = new GraphPropertyFloat(0.8f);

        [SerializeField]
        GraphEvent _onClickEvent;

        Vector2 _gridUnit;
        Rect[] _segmentRects;

        public int Column { get { return _column.Value; } set { _column.Value = value; } }
        public int Row { get { return _row.Value; } set { _row.Value = value; } }
        public float MinValue { get { return _minValue.Value; } set { _minValue.Value = value; } }
        public float MaxValue { get { return _maxValue.Value; } set { _maxValue.Value = value; } }
        public int GridSize { get { return _gridSize.Value; } set { _gridSize.Value = value; } }
        public Color GridColor { get { return _gridColor.Value; } set { _gridColor.Value = value; } }
        public float WidthRatio { get { return _widthRatio.Value; } set { _widthRatio.Value = value; } }

        public Vector2 GridUnit { get { return _gridUnit; } }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            _gridUnit = Vector2.zero;
            _gridUnit.x = 0f;
            _gridUnit.y = rectTransform.rect.size.y / Column;
            if(_content.Values.Length != 0)
            {
                _gridUnit.x = rectTransform.rect.size.x / _content.Values.Length;
            }

            var rect = rectTransform.rect;
            for (int i = 0; i <= Column; ++i)
            {
                DrawRect(vh, new Rect(rect.xMin, rect.yMin + i * _gridUnit.y - GridSize, rect.size.x, GridSize), GridColor);
            }

            // グリッド
            for (int i = 0; i < Row + 1; ++i)
            {
                var x = rect.xMin + GridUnit.x * i;
                DrawRect(vh, new Rect(x - GridSize / 2f, rect.yMin, GridSize, rect.height), GridColor);
            }

            // 中心
            for (int i = 0; i < Row; ++i)
            {
                var x = rect.xMin + GridUnit.x * i + GridUnit.x / 2;
                DrawRect(vh, new Rect(x - GridSize / 2f, rect.yMin, 1f, rect.height), GridColor);
            }

            var segmentUnit = GridUnit;
            var widthRatio = WidthRatio;
            var maxX = _content.Values.Length;
            var maxValue = MaxValue;
            var minValue = MinValue;

            _segmentRects = new Rect[maxX];
            for (int xIndex = 0; xIndex < maxX; ++xIndex)
            {
                var content = _content.Values[xIndex];
                var width = segmentUnit.x * widthRatio;
                var segmentUnitXMin = rect.xMin + segmentUnit.x * xIndex;
                _segmentRects[xIndex] = Rect.MinMaxRect(segmentUnitXMin, rect.yMin, segmentUnitXMin + segmentUnit.x, rect.yMax);

                var x = segmentUnitXMin + segmentUnit.x / 2f;
                var value = content.Value;
                var yUnit = rect.size.y / (maxValue - minValue);
                var yGround = rect.yMin + yUnit * -minValue;
                var yMax = Mathf.Clamp(rect.yMin + yUnit * (value - minValue), rect.yMin, rect.yMax);
                var yMin = Mathf.Clamp(yGround, rect.yMin, rect.yMax);
                DrawRect(vh, Rect.MinMaxRect(x - width / 2f, yMin, x + width / 2, yMax), content.Color);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var position = ScreenPositionToLocalPosition(eventData.position);

            // セグメント自体の判定
            for(int i = 0; i < _content.Values.Length;++i)
            {
                if (_segmentRects[i].Contains(position))
                {
                    _onClickEvent.Invoke(i);
                    Debug.Log($"Click Segment {i}");
                }
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnUpdateProperty();
        }
#endif
    }
}
