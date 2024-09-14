#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AsteroidGameManager.GoldenAsteroidData))]
public class PD_GoldAsteroidSpawn : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Calculate the widths for the fields
        //float halfWidth = position.width / 2;
        float wideWidth = position.width * .7f;
        float smallWidth = position.width - wideWidth;

        // Get properties
        SerializedProperty timeProperty = property.FindPropertyRelative("gameProgress");
        SerializedProperty positionProperty = property.FindPropertyRelative("position");

        // Create Rects for the fields
        Rect timeRect = new Rect(position.x, position.y, smallWidth - 5, position.height);
        Rect positionRect = new Rect(position.x + smallWidth + 5, position.y, wideWidth - 5, position.height);

        // Draw fields
        EditorGUI.PropertyField(timeRect, timeProperty, GUIContent.none);
        EditorGUI.PropertyField(positionRect, positionProperty, GUIContent.none);
    }
}

#endif