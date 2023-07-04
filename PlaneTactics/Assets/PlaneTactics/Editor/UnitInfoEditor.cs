using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitInfo))]
[CanEditMultipleObjects]
public class UnitInfoEditor : Editor
{
	protected SerializedProperty nameProp;
	protected SerializedProperty unitClassProp;
	protected SerializedProperty modelProp;

	protected SerializedProperty maxHealthProp;
	protected SerializedProperty hitboxRadiusProp;
	protected SerializedProperty evadeProp;

	protected SerializedProperty moveAreaDistancesProp;
	protected SerializedProperty moveAreaNearAnglesProp;
	protected SerializedProperty moveAreaFarAnglesProp;
	protected SerializedProperty moveAdjustAccelerationProp;
	protected SerializedProperty moveAdjustAgilityProp;

	protected SerializedProperty attackDistanceProp;
	protected SerializedProperty attackMaxAngleProp;
	protected SerializedProperty attackPowerProp;
	protected SerializedProperty attackRollsProp;
	protected SerializedProperty attackBaseProbabilityProp;

	void OnEnable()
	{
		nameProp = serializedObject.FindProperty("name");
		unitClassProp = serializedObject.FindProperty("unitClass");
		modelProp = serializedObject.FindProperty("model");

		maxHealthProp = serializedObject.FindProperty("maxHealth");
		hitboxRadiusProp = serializedObject.FindProperty("hitboxRadius");
		evadeProp = serializedObject.FindProperty("evade");

		moveAreaDistancesProp = serializedObject.FindProperty("moveArea").FindPropertyRelative("distances");
		moveAreaNearAnglesProp = serializedObject.FindProperty("moveArea").FindPropertyRelative("nearAngles");
		moveAreaFarAnglesProp = serializedObject.FindProperty("moveArea").FindPropertyRelative("farAngles");
		moveAdjustAccelerationProp = serializedObject.FindProperty("moveAdjust").FindPropertyRelative("acceleration");
		moveAdjustAgilityProp = serializedObject.FindProperty("moveAdjust").FindPropertyRelative("agility");

		attackDistanceProp = serializedObject.FindProperty("attackDistance");
		attackMaxAngleProp = serializedObject.FindProperty("attackMaxAngle");
		attackPowerProp = serializedObject.FindProperty("attackPower");
		attackRollsProp = serializedObject.FindProperty("attackRolls");
		attackBaseProbabilityProp = serializedObject.FindProperty("attackBaseProbability");
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

		EditorGUILayout.LabelField(new GUIContent("Basic Information"), EditorStyles.boldLabel); 
		EditorGUILayout.PropertyField(nameProp, new GUIContent("Name"));
		EditorGUILayout.PropertyField(unitClassProp, new GUIContent("Unit Class"));
		EditorGUILayout.PropertyField(modelProp, new GUIContent("Model"));

		EditorGUILayout.LabelField(new GUIContent("Defence Properties"), EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(maxHealthProp, new GUIContent("Max Health"));
		EditorGUILayout.PropertyField(hitboxRadiusProp, new GUIContent("Hitbox Radius"));
		EditorGUILayout.Slider(evadeProp, 0, 1, new GUIContent("Evade"));

		EditorGUILayout.LabelField(new GUIContent("Movement Properties"), EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(moveAreaDistancesProp, new GUIContent("Move Distances"));
		EditorGUILayout.PropertyField(moveAreaNearAnglesProp, new GUIContent("Move Near Angles"));
		EditorGUILayout.PropertyField(moveAreaFarAnglesProp, new GUIContent("Move Far Angles"));
		EditorGUILayout.Slider(moveAdjustAccelerationProp, 0, 1, new GUIContent("Acceleration"));
		EditorGUILayout.Slider(moveAdjustAgilityProp, 0, 1, new GUIContent("Agility"));

		EditorGUILayout.LabelField(new GUIContent("Movement Properties"), EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(attackDistanceProp, new GUIContent("Attack Distances"));
		EditorGUILayout.PropertyField(attackMaxAngleProp, new GUIContent("Attack Max Angle"));
		EditorGUILayout.PropertyField(attackPowerProp, new GUIContent("Attack Power"));
		EditorGUILayout.PropertyField(attackRollsProp, new GUIContent("Attack Rolls"));
		EditorGUILayout.Slider(attackBaseProbabilityProp, 0, 1, new GUIContent("Attack Base Probability"));

		// save changes
		if (EditorGUI.EndChangeCheck())
		{
			SceneView.RepaintAll();
		}
		serializedObject.ApplyModifiedProperties();
	}
}
