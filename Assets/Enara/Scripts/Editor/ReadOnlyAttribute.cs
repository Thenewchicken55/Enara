using UnityEditor;
using UnityEngine;

namespace Enara.Editor
{
    /// <summary>
    /// Marks a serialized field as read-only in the Inspector. Useful for showing state that
    /// other code sets (e.g. currentChapterId, IsHealed) without letting designers edit it.
    ///
    /// Usage: <c>[ReadOnly] [SerializeField] private float x;</c>
    /// </summary>
    public sealed class ReadOnlyAttribute : PropertyAttribute { }

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public sealed class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
