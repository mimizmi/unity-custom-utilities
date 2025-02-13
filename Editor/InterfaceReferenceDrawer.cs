using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace Mimizh.UnityUtilities.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    [CustomPropertyDrawer(typeof(InterfaceReference<,>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        const string UnderlyingValueFieldName = "underlyingValue";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var underlyingProperty = property.FindPropertyRelative(UnderlyingValueFieldName);
            var args = GetArguments(fieldInfo);
            
            EditorGUI.BeginProperty(position, label, property);
            var assignedObject = EditorGUI.ObjectField(position, label, underlyingProperty.objectReferenceValue, typeof(Object), true);

            if (assignedObject != null)
            {
                if (assignedObject is GameObject gameObject)
                {
                    ValidateAndAssignObject(underlyingProperty, gameObject.GetComponent(args.InterfaceType), gameObject.name, args.InterfaceType.Name);
                }
                else
                {
                    ValidateAndAssignObject(underlyingProperty, assignedObject, args.InterfaceType.Name);
                }
            }
            else
            {
                underlyingProperty.objectReferenceValue = null;
            }
            EditorGUI.EndProperty();
            InterfaceReferenceUtil.OnGUI(position, underlyingProperty, label, args);
        }

        static InterfaceArgs GetArguments(FieldInfo fieldInfo)
        {
            Type objectType = null, interfaceType = null;
            Type fieldType = fieldInfo.FieldType;

            bool TryGetTypesFromInterfaceReference(Type type, out Type objType, out Type intfType)
            {
                objType = intfType = null;
                
                if (type?.IsGenericType != true) return false;
                
                var genericType = type.GetGenericTypeDefinition();
                if (genericType == typeof(InterfaceReference<>)) type = type.BaseType;
                
                if (type?.GetGenericTypeDefinition() == typeof(InterfaceReference<,>))
                {
                    var types = type.GetGenericArguments();
                    intfType = types[0];
                    objType = types[1];
                    return true;
                }
                return false;
            }

            void GetTypesFromList(Type type, out Type objType, out Type intfType)
            {
                objType = intfType = null;
                
                var listInterfaces = type.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));

                if (listInterfaces != null)
                {
                    var elementType = listInterfaces.GetGenericArguments()[0];
                    TryGetTypesFromInterfaceReference(elementType, out objType, out intfType);
                }
            }

            if (!TryGetTypesFromInterfaceReference(fieldType, out objectType, out interfaceType))
            {
                GetTypesFromList(fieldType, out objectType, out interfaceType);
            }
            
            return new InterfaceArgs(objectType, interfaceType);
        }

        static void ValidateAndAssignObject(SerializedProperty property, Object targetObject, string componentNameOrType,
            string interfaceName = null)
        {
            if (targetObject != null)
            {
                property.objectReferenceValue = targetObject;
            }
            else
            {
                Debug.LogWarning(
                    @$"The {(interfaceName != null
                            ? $"GameObject '{componentNameOrType}'"
                            : $"assigned object")} does not have a component that implements '{componentNameOrType}'." 
                    );
                property.objectReferenceValue = null;
            }
        }
        
    }
    
    public struct InterfaceArgs
    {
        public readonly Type ObjectType;
        public readonly Type InterfaceType;

        public InterfaceArgs(Type objectType, Type interfaceType)
        {
            Debug.Assert(typeof(Object).IsAssignableFrom(objectType), $"{nameof(objectType)} need to be assignable from {typeof(Object)}");
            Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} needs to be an interface");
            
            ObjectType = objectType;
            InterfaceType = interfaceType;
        }
    }
}