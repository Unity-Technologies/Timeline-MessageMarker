using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Message)), CanEditMultipleObjects]
public class MessageInspector : Editor
{
    const string k_OverloadWarning = "Some functions were overloaded in MonoBehaviour components and may not work as intended if used with Animation Events!";
    const string k_NoFunction = "No function";
    const string k_FunctionLabel = "Function: ";
    const string k_MethodIsNotValid = "Method is not valid. Add behaviours to Timeline GameObject with simple functions (they should show here).";
    
    SerializedProperty m_Time;
    SerializedProperty m_Method;
    SerializedProperty m_Retroactive;
    SerializedProperty m_EmitOnce;
    SerializedProperty m_ArgumentType;
    SerializedProperty m_IntArg;
    SerializedProperty m_StringArg;
    SerializedProperty m_ObjectArg;
    SerializedProperty m_FloatArg;

    void OnEnable()
    {
        m_Time = serializedObject.FindProperty("m_Time");
        m_Method = serializedObject.FindProperty("method");
        m_ArgumentType = serializedObject.FindProperty("parameterType");
        m_IntArg = serializedObject.FindProperty("Int");
        m_StringArg = serializedObject.FindProperty("String");
        m_ObjectArg = serializedObject.FindProperty("Object");
        m_FloatArg = serializedObject.FindProperty("Float");
        m_Retroactive = serializedObject.FindProperty("retroactive");
        m_EmitOnce = serializedObject.FindProperty("emitOnce");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var marker = target as Marker;
        var parent = marker.parent;
        var boundObj = TimelineEditor.inspectedDirector.GetGenericBinding(parent);

        using (var changeScope = new EditorGUI.ChangeCheckScope())
        {
            EditorGUILayout.PropertyField(m_Time);

            DrawMethodAndArguments(GetGameObject(boundObj));

            EditorGUILayout.PropertyField(m_Retroactive);
            EditorGUILayout.PropertyField(m_EmitOnce);
            
            if (changeScope.changed)
                serializedObject.ApplyModifiedProperties();
        }
    }

    void DrawMethodAndArguments(GameObject boundGO)
    {
        var supportedMethods = Utilities.Methods.CollectSupportedMethods(boundGO).ToList();
        var dropdown = supportedMethods.Select(i => i.ToString()).ToList();
        dropdown.Add(k_NoFunction);

        var selectedMethodId = supportedMethods.FindIndex(i => i.name == m_Method.stringValue);
        if (selectedMethodId == -1)
            selectedMethodId = supportedMethods.Count;

        var previousMixedValue = EditorGUI.showMixedValue;
        {
            if (m_Method.hasMultipleDifferentValues)
                EditorGUI.showMixedValue = true;
            selectedMethodId = EditorGUILayout.Popup(k_FunctionLabel, selectedMethodId, dropdown.ToArray());
        }
        EditorGUI.showMixedValue = previousMixedValue;
        
        if (selectedMethodId < supportedMethods.Count)
        {
            var method = supportedMethods.ElementAt(selectedMethodId);
            m_Method.stringValue = method.name;
            DrawArguments(method);
            if (supportedMethods.Any(i => i.isOverload == true))
                EditorGUILayout.HelpBox(k_OverloadWarning, MessageType.Warning, true);
        }
        else
            EditorGUILayout.HelpBox(k_MethodIsNotValid, MessageType.Warning, true);
    }

    static GameObject GetGameObject(Object obj)
    {
        if (obj as GameObject != null)
            return (GameObject)obj;
        if (obj as Component != null)
            return ((Component)obj).gameObject;
        return null;
    }

    void DrawArguments(MethodDesc method)
    {
        m_ArgumentType.enumValueIndex = (int)method.type;
        switch (method.type)
        {
            case ParameterType.Int:
            {
                EditorGUILayout.PropertyField(m_IntArg);
                break;
            }
            case ParameterType.Float:
            {
                EditorGUILayout.PropertyField(m_FloatArg);
                break;
            }
            case ParameterType.Object:
            {
                EditorGUILayout.PropertyField(m_ObjectArg);
                break;
            }
            case ParameterType.String:
            {
                EditorGUILayout.PropertyField(m_StringArg);
                break;
            }
            default:
            case ParameterType.None: break;
        }
    }
}
