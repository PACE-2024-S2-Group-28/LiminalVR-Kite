#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ScriptableObjects;
using UnityEngine.UIElements;

[CustomEditor(typeof(SoundScripObj))]
public class SoundScriptObjEditor : Editor
{
    private Color playColor = Color.green/1.5f;
    private Color stopColor = Color.red/1.5f;



    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SoundScripObj myScript = (SoundScripObj)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Semitone Conversion")) myScript.SyncPitchAndSemitones();
        //EditorGUILayout.Space();

        GUIStyle playButtonStyle = new GUIStyle(GUI.skin.button);
        playButtonStyle.fixedHeight = 80;
        playButtonStyle.fontSize = 22;
        playButtonStyle.normal.background = CreateTexture(playColor);
        playButtonStyle.hover.background = CreateTexture(playColor/2);

        GUIStyle stopButtonStyle = new GUIStyle(playButtonStyle);
        stopButtonStyle.normal.background = CreateTexture(stopColor);
        stopButtonStyle.hover.background = CreateTexture(stopColor/2);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play Preview", playButtonStyle)) myScript.PlayPreview();
        if (GUILayout.Button("Stop Preview", stopButtonStyle)) myScript.StopPreview();
        EditorGUILayout.EndHorizontal();
    }

    private Texture2D CreateTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}

#endif
