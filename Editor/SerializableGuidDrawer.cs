using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
namespace Mimizh.UnityUtilities.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    public class SerializableGuidDrawer : PropertyDrawer
    {
        static readonly string[] GuidParts = { "Part1", "Part2", "Part3", "Part4" };
        private const float ButtonWidth = 22f;
        private const float Spacing = 2f;
        private GUIStyle guidLabelStyle;
        private readonly Color validGuidColor = new Color(0.3f, 1f, 0.3f, 0.3f);
        private readonly Color invalidGuidColor = new Color(1f, 0.2f, 0.2f, 0.5f);

        static SerializedProperty[] GetGuidParts(SerializedProperty property)
        {
            var values = new SerializedProperty[GuidParts.Length];
            for (int i = 0; i < GuidParts.Length; i++)
            {
                values[i] = property.FindPropertyRelative(GuidParts[i]);
            }

            return values;
        }

        // 新增方法：检查GUID是否有效（至少有一个部分不为零）
        static bool IsGuidValid(SerializedProperty[] guidParts)
        {
            return guidParts.Any(part => part != null && part.uintValue != 0);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Initialize styles if needed
            if (guidLabelStyle == null)
            {
                guidLabelStyle = new GUIStyle(EditorStyles.textField)
                {
                    alignment = TextAnchor.MiddleLeft
                };
            }

            // Calculate rects - first draw the prefix label
            Rect labelRect = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            // Then calculate the remaining rects for the GUID field and button
            Rect buttonRect = new Rect(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);
            Rect guidRect = new Rect(labelRect.x, labelRect.y, labelRect.width - ButtonWidth - Spacing, labelRect.height);

            // 获取GUID部分并检查是否有效
            var guidParts = GetGuidParts(property);
            
            bool isGuidValid = IsGuidValid(guidParts);
            
            // 绘制GUID显示区域
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = isGuidValid ? validGuidColor : invalidGuidColor;
            
            if (guidParts.All(x => x != null))
            {
                string guidText = isGuidValid 
                    ? FormatGuidString(BuildGuidString(guidParts))
                    : "Empty GUID";
                EditorGUI.SelectableLabel(guidRect, guidText, guidLabelStyle);
            }
            else
            {
                EditorGUI.SelectableLabel(guidRect, "GUID Not Initialized", guidLabelStyle);
            }
            
            GUI.backgroundColor = originalColor;

            // 绘制菜单按钮
            if (GUI.Button(buttonRect, new GUIContent("⋮", "GUID Options"), EditorStyles.miniButton))
            {
                ShowContextMenu(property);
            }

            // 处理右键点击
            bool hasClicked = Event.current.type == EventType.MouseUp  && Event.current.button == 1;
            if (hasClicked && position.Contains(Event.current.mousePosition)) {
                ShowContextMenu(property);
                Event.current.Use();
            }

            EditorGUI.EndProperty();
        }

        private string FormatGuidString(string guid)
        {
            if (guid.Length != 32) return guid;
            
            // Format as standard GUID: 8-4-4-4-12
            return guid.Substring(0, 8) + "-" + 
                   guid.Substring(8, 4) + "-" + 
                   guid.Substring(12, 4) + "-" + 
                   guid.Substring(16, 4) + "-" + 
                   guid.Substring(20, 12);
        }

        void ShowContextMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();
            var guidParts = GetGuidParts(property);
            bool isGuidValid = IsGuidValid(guidParts);
            
            if (guidParts.All(x => x != null))
            {
                if (isGuidValid)
                {
                    menu.AddItem(new GUIContent("Copy GUID/Raw Format"), false, () => CopyGuid(property, false));
                    menu.AddItem(new GUIContent("Copy GUID/Standard Format (with hyphens)"), false, () => CopyGuid(property, true));
                    menu.AddSeparator("");
                }
                
                menu.AddItem(new GUIContent("Generate New GUID"), false, () => RegenerateGuid(property));
                
                if (isGuidValid)
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Reset GUID"), false, () => ResetGuid(property));
                }
            }
            else
            {
                menu.AddItem(new GUIContent("Generate New GUID"), false, () => RegenerateGuid(property));
            }
            
            menu.ShowAsContext();
        }

        void CopyGuid(SerializedProperty property, bool formatted)
        {
            var guidParts = GetGuidParts(property);
            if (guidParts.Any(x => x == null)) return;

            string guid = BuildGuidString(guidParts);
            if (formatted)
            {
                guid = FormatGuidString(guid);
            }
            
            EditorGUIUtility.systemCopyBuffer = guid;
            Debug.Log($"GUID copied to clipboard: {guid}");
        }

        void ResetGuid(SerializedProperty property)
        {
            const string warning = "Are you sure you want to reset the GUID?";
            if (!EditorUtility.DisplayDialog("Reset GUID", warning, "Yes", "No")) return;

            foreach (var part in GetGuidParts(property))
            {
                part.uintValue = 0;
            }

            property.serializedObject.ApplyModifiedProperties();
            Debug.Log("GUID has been reset.");
        }

        void RegenerateGuid(SerializedProperty property)
        {
            var guidParts = GetGuidParts(property);
            bool isGuidValid = IsGuidValid(guidParts);
            
            if (isGuidValid)
            {
                const string warning = "Are you sure you want to regenerate the GUID?";
                if (!EditorUtility.DisplayDialog("Regenerate GUID", warning, "Yes", "No")) return;
            }

            byte[] bytes = Guid.NewGuid().ToByteArray();
            SerializedProperty[] parts = GetGuidParts(property);

            for (int i = 0; i < GuidParts.Length; i++)
            {
                parts[i].uintValue = BitConverter.ToUInt32(bytes, i * 4);
            }

            property.serializedObject.ApplyModifiedProperties();
            Debug.Log("GUID has been regenerated.");
        }

        static string BuildGuidString(SerializedProperty[] guidParts)
        {
            return new StringBuilder()
                .AppendFormat("{0:X8}", guidParts[0].uintValue)
                .AppendFormat("{0:X8}", guidParts[1].uintValue)
                .AppendFormat("{0:X8}", guidParts[2].uintValue)
                .AppendFormat("{0:X8}", guidParts[3].uintValue)
                .ToString();
        }
    }
}