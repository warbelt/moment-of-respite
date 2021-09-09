using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Editor
{
    [CustomPropertyDrawer(typeof(EnemySpawnInfo))]
    public class EnemySpawnInfoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enemyPrefabProperty = property.FindPropertyRelative("enemyPrefab");
            var minRoundProperty = property.FindPropertyRelative("minRound");
            var maxRoundProperty = property.FindPropertyRelative("maxRound");

            position.width -= 66;
            EditorGUI.PropertyField(position, enemyPrefabProperty, label, true);

            position.x = position.width + 52;
            position.width = 24;
            EditorGUI.PropertyField(position, minRoundProperty, GUIContent.none);

            position.x += 32;
            EditorGUI.PropertyField(position, maxRoundProperty, GUIContent.none);

        }
    }
}
