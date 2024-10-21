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
        float labelWidth = position.width * .2f;
        float wideWidth = (position.width - labelWidth*2f) * .8f;
        float smallWidth = (position.width - labelWidth*2f) - wideWidth;

        // Get properties
        SerializedProperty timeProperty = property.FindPropertyRelative("spawnTime");
        SerializedProperty positionProperty = property.FindPropertyRelative("position");

        #region Old method without labels
        //// Create Rects for the fields
        //Rect timeRect = new Rect(position.x, position.y, smallWidth - 5, position.height);
        //Rect positionRect = new Rect(position.x + smallWidth + 5, position.y, wideWidth - 5, position.height);

        //// Draw fields
        //EditorGUI.PropertyField(timeRect, timeProperty, GUIContent.none);
        //EditorGUI.PropertyField(positionRect, positionProperty, GUIContent.none);
        #endregion

        //new method with labels

        // Create Rects for the labels and fields
        Rect timeLabelRect = new Rect(position.x, position.y, labelWidth, position.height);
        Rect timeRect = new Rect(position.x + labelWidth, position.y, smallWidth, position.height);
        Rect positionLabelRect = new Rect(position.x + smallWidth + labelWidth, position.y, labelWidth, position.height);
        Rect positionRect = new Rect(position.x + smallWidth + labelWidth * 2, position.y, wideWidth, position.height);

        // Draw labels and field
        EditorGUI.LabelField(timeLabelRect, "spawnTime");
        EditorGUI.PropertyField(timeRect, timeProperty, GUIContent.none);
        EditorGUI.LabelField(positionLabelRect, "spawnPos");
        EditorGUI.PropertyField(positionRect, positionProperty, GUIContent.none);
    }
}

#endif