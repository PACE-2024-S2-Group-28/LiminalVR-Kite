#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ScriptableObjects;
using UnityEngine.UIElements;

using UnityEngine.Events;

[CustomEditor(typeof(AudioAssigningSounds)), CanEditMultipleObjects]
public class AudioAssigningSoundsEditor : Editor
{
    //private Color playColor = Color.green / 1.5f;
    //private Color stopColor = Color.red / 1.5f;

    SerializedProperty sEventSource;
    SerializedProperty sEventGroups;

    private void OnEnable()
    {
        sEventSource = serializedObject.FindProperty("eventSource");
        sEventGroups = serializedObject.FindProperty("eventGroups");
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        AudioAssigningSounds myScript = (AudioAssigningSounds)target;

        //float width30p = (EditorGUIUtility.currentViewWidth - 20) * .3f;
        //float width70p = (EditorGUIUtility.currentViewWidth - 20) * .7f;

        EditorGUILayout.Space();
        //EditorGUILayout.BeginHorizontal();
        //myScript.eventSource = (MyEvents)EditorGUILayout.ObjectField("Event Source", myScript.eventSource,
        //                            typeof(MyEvents), true);
        EditorGUILayout.PropertyField(sEventSource, new GUIContent("Event Source"));
        //EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField(" - ensure reference to `AudioEventsGlobal` on same object");
        EditorGUILayout.LabelField(" - above is required");
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        if (GUILayout.Button("Get Events")) myScript.GetEventNames();
        EditorGUILayout.Space();

        //EditorGUILayout.BeginHorizontal();
        //    EditorGUILayout.LabelField("Default Sound", GUILayout.Width(width30p));
        //    myScript.defaultSound = (SoundScripObj)EditorGUILayout.ObjectField(myScript.defaultSound, 
        //                                typeof(SoundScripObj), false, GUILayout.Width(width70p));
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.LabelField(" - above is required");
        //EditorGUILayout.Space();

        EditorGUILayout.LabelField("Assign a Sound to Events", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        //if (myScript.eventGroups.Count == 0)
        //    EditorGUILayout.LabelField("Error: No events found to assign a sound to");
        //else
        //{
        //    //drawing the list fields one by one so they can be side by side
        //    foreach (var group in myScript.eventGroups)
        //    {
        //        EditorGUILayout.BeginHorizontal();

        //        //EditorGUI.BeginDisabledGroup(true);
        //        //    group.eventName = EditorGUILayout.TextField(group.eventName);
        //        //EditorGUI.EndDisabledGroup();
        //        group.soundObj = (SoundScripObj)EditorGUILayout.ObjectField(group.eventName, group.soundObj, typeof(SoundScripObj), false);

        //        EditorGUILayout.EndHorizontal();
        //    }
        //}

        if (sEventGroups.arraySize == 0)
            EditorGUILayout.LabelField("Error: No events found to assign a sound to");
        else
        {
            for (int i = 0; i < sEventGroups.arraySize; i++)
            {
                var elementProp = sEventGroups.GetArrayElementAtIndex(i);
                var soundObjProp = elementProp.FindPropertyRelative("soundObj");
                var eventNameProp = elementProp.FindPropertyRelative("eventName");

                EditorGUILayout.PropertyField(soundObjProp, new GUIContent(eventNameProp.stringValue));
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Clear Events")) myScript.ClearEvents();
        EditorGUILayout.Space();

        //DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed)
            EditorUtility.SetDirty(myScript);
    }

    //private Texture2D CreateTexture(Color color)
    //{
    //    Texture2D texture = new Texture2D(1, 1);
    //    texture.SetPixel(0, 0, color);
    //    texture.Apply();
    //    return texture;
    //}
}

#endif