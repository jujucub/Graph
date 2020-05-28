using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Graph
{
    [System.Serializable]
    public class GraphEvent : UnityEvent<int> { }

    public class GraphRenderer : Graphic
    {
        [SerializeField]
        protected GraphContent _content;

        protected bool _isDirty;

        public GraphContent Content { get { return _content; } set { _content = value; SetupContentProperty(); } }

        protected override void Start()
        {
            var type = GetType();
            var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var value = field.GetValue(this);
                if(value is IGraphProperty)
                {
                    var property = value as IGraphProperty;
                    property.OnValueChange += OnUpdateProperty;
                }
            }
        }

        protected void SetupContentProperty()
        {
            var valueType = typeof(GraphValue);
            var fields = valueType.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            foreach (var content in _content.Values)
            {
                foreach (var field in fields)
                {
                    var value = field.GetValue(content);
                    if (value is IGraphProperty)
                    {
                        var property = value as IGraphProperty;
                        property.OnValueChange += OnUpdateProperty;
                    }
                }
            }
        }

        protected void DrawRect(VertexHelper vh, Rect rect, Color color = default(Color))
        {
            vh.AddUIVertexQuad(new UIVertex[] {
                new UIVertex() { position = new Vector3(rect.xMin, rect.yMin), color = color },
                new UIVertex() { position = new Vector3(rect.xMin, rect.yMax), color = color },
                new UIVertex() { position = new Vector3(rect.xMax, rect.yMax), color = color },
                new UIVertex() { position = new Vector3(rect.xMax, rect.yMin), color = color },
            });
        }

        protected virtual void OnUpdateProperty()
        {
            _isDirty = true;
            SetVerticesDirty();
        }

        protected Vector2 ScreenPositionToLocalPosition(Vector2 screenPosition)
        {
            return rectTransform.InverseTransformPoint(screenPosition);
        }
    }
}
