using UnityEngine;
using UnityEditor;

public class GraphTest : MonoBehaviour
{
    [SerializeField]
    Graph.GraphBarRenderer _graphRenderer;

    private void Start()
    {
        _graphRenderer.Content = new Graph.GraphContent()
        {
            Values = new Graph.GraphValue[] {
                new Graph.GraphValue(){ Value = 10, Color = Color.red },
                new Graph.GraphValue(){ Value = 5, Color = Color.red },
                new Graph.GraphValue(){ Value = 3, Color = Color.red },
            }
        };
    }

    private void Update()
    {
        for(int i = 0; i < _graphRenderer.Content.Values.Length; ++i)
        {
            _graphRenderer.Content.Values[i].Value++;
        }
    }
}