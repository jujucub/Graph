using UnityEngine;
using UnityEditor;

namespace Graph
{
    public static class ActionExtensions
    {
        public static void SafeInvoke(this System.Action action)
        {
            if (action == null) return;
            action.Invoke();
        }
    }
}