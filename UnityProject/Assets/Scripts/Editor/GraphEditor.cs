using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var graph = target as Graph;
        base.OnInspectorGUI();
    }
}
