using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGameManager))]
public class Editor_AsteroidGameManager : Editor
{
    private AsteroidGameManager targetManager;
    //SerializedProperty prop_difficultyCurve;
    //SerializedProperty prop_goldAsteroidSpawns;
    //SerializedProperty prop_gameLength;
    
    private void OnEnable()
    {
        targetManager = (AsteroidGameManager)target;
        //prop_gameLength = serializedObject.FindProperty("GameLength");
        //prop_difficultyCurve = serializedObject.FindProperty("DifficultyCurve");
        //prop_goldAsteroidSpawns = serializedObject.FindProperty("GoldSpawns");

        EditorApplication.update += UpdatePreview;
    }

    private void OnDisable()
    {
        EditorApplication.update -= UpdatePreview;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Draw the combined animation curves
        GUILayout.Label("Difficulty Curve and Events", EditorStyles.boldLabel);
        float margin = 20;
        float width = EditorGUIUtility.currentViewWidth - margin*2;

        //GUILayout.BeginHorizontal();
        //GUILayout.FlexibleSpace();
        Rect curveRect = GUILayoutUtility.GetRect(width, width*.5f, GUILayout.MaxWidth(width), GUILayout.MaxHeight(width*.5f)); // The area for the graph
        //curveRect.x += margin;

        if (Event.current.type == EventType.Repaint) {
            // Background of the curve area
            EditorGUI.DrawRect(curveRect, Color.black *.75f);

            // Draw the difficulty curve
            Handles.color = Color.green;
            DrawCurve(curveRect, targetManager.DifficultyCurve, Color.green, targetManager.GameLength);

            // Draw the current time spike
            float time;
            if (Application.isPlaying) time = Time.time;
            else time = 0f;
            Handles.color = Color.red;
            DrawSpike(curveRect, time, targetManager.GameLength, Color.red);

            // Draw timed events as spikes
            Handles.color = Color.yellow;
            foreach (var spawn in targetManager.GoldSpawns) {
                DrawSpike(curveRect, spawn.gameProgress, targetManager.GameLength, Color.yellow);
            }
        }
    }

    private void UpdatePreview()
    {
        Repaint();
    }

    float outerMargin = 20f;
    float innerMargin = 10f;

    private void DrawCurve(Rect rect, AnimationCurve curve, Color color, float gameDuration)
    {
        Handles.BeginGUI();
        Handles.color = color;

        innerMargin = 10f;

        int resolution = 30;
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++) {
            float t = (float)i / (resolution - 1);
            float value = curve.Evaluate(t);

            float xPos = Mathf.Lerp(rect.x + innerMargin, rect.x+rect.width - innerMargin*2f, t);
            float yPos = Mathf.Lerp(rect.y+rect.height - innerMargin, rect.y+innerMargin*2f, value);
            //float xPos = Mathf.Lerp(rect.x, rect.xMax, t); // Spread X across the full rect width
            //float yPos = Mathf.Lerp(rect.yMax, rect.y, value); // Fit Y within rect height

            points[i] = new Vector3(xPos, yPos, 0);
        }

        Handles.DrawAAPolyLine(3, points);
        Handles.EndGUI();
    }

    private void DrawSpike(Rect rect, float time, float gameDuration, Color color)
    {
        float normalizedTime = Mathf.Clamp01(time / gameDuration);
        float xPos = Mathf.Lerp(rect.x + innerMargin, rect.xMax - innerMargin*2f, normalizedTime);

        Handles.DrawLine(new Vector3(xPos, rect.y+innerMargin, 0), new Vector3(xPos, rect.yMax-innerMargin, 0));
    }
}
