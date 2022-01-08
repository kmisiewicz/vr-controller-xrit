using UnityEditor;
using UnityEngine;

namespace Chroma.Utility.Editor
{
    [CustomPropertyDrawer(typeof(ChromaBarAttribute))]
    public class ChromaBarDrawer : DecoratorDrawer
    {
        public override float GetHeight() => 10;

        public override void OnGUI(Rect position)
        {
            position.y += 5;
            position.height = 2;
            position.x -= 2;
            float d = position.width = (position.width + 6) / 3;
            EditorGUI.DrawRect(position, new Color(0.8f, 0f, 0f));
            position.x += d;
            EditorGUI.DrawRect(position, new Color(0.8f, 0.8f, 0f));
            position.x += d;
            EditorGUI.DrawRect(position, new Color(0f, 0f, 0.8f));
        }
    }
}