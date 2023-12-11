#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.goldsprite.gstools.CustomRequireEssentials
{
    [CustomPropertyDrawer(typeof(RequireEssentialsAttribute))]
    public class RequireEssentialsDrawer : PropertyDrawer
    {
        public float panelHeight;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //return base.GetPropertyHeight(property, label);
            return panelHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
#if !UNITY_2021 && UNITY_2021_3_OR_NEWER
            var target = (MonoBehaviour)property.serializedObject.targetObject;
            FieldInfo field = (target.GetType()).GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var attr = (RequireEssentialsAttribute)Attribute.GetCustomAttribute(field, typeof(RequireEssentialsAttribute));
            Type[] types = attr.type;

            bool pass = true;
            foreach (var type in types)
            {
                if (target.GetComponent(type) != null) continue;

                pass = false;
                target.enabled = false;
                EditorGUILayout.HelpBox("必需的组件不存在，点击按钮来添加。", MessageType.Warning);
                if (GUILayout.Button("添加 " + type.Name))
                    if (target.gameObject.AddComponent(type) == null)
                        Debug.Log("添加组件失败, 请手动添加.");
            }
            if (pass)
                target.enabled = true;
#else
            panelHeight = 0;
            var lineHeight = EditorGUIUtility.singleLineHeight * 1.6f;
            var lineSpcing = lineHeight + EditorGUIUtility.singleLineHeight / 3f;

            var target = (MonoBehaviour)property.serializedObject.targetObject;
            FieldInfo field = (target.GetType()).GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var attr = (RequireEssentialsAttribute)Attribute.GetCustomAttribute(field, typeof(RequireEssentialsAttribute));
            Type[] types = attr.type;

            bool pass = true;
            foreach (var type in types)
            {
                if (target.GetComponent(type) != null) continue;

                pass = false;
                target.enabled = false;

                position.height = lineHeight;
                EditorGUI.HelpBox(position, "必需的组件不存在，点击按钮来添加。", MessageType.Warning);
                position.y += lineSpcing;

                position.height = lineHeight / 1.5f;
                if (GUI.Button(position, "添加 " + type.Name))
                    if (target.gameObject.AddComponent(type) == null)
                        Debug.Log("添加组件失败, 请手动添加.");
                position.y += lineSpcing;

                panelHeight += lineHeight + lineSpcing;
            }
            if (pass)
                target.enabled = true;
#endif

        }


    }


}
#endif