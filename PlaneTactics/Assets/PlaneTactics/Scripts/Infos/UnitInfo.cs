using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Plane Fight/Unit Info", order = 1)]
public class UnitInfo : ScriptableObject
{
	[Header("Basic Information")]
	public new string name = "New Plane";
	public UnitClass unitClass = UnitClass.LightFighter;
	public GameObject model = null;

	[Header("Defence Properties")]
	public float maxHealth = 100f;
	public float hitboxRadius = 1.5f;
	public float evade = 0.8f;

	[Header("Movement Properties")]
	public MoveArea moveArea = new MoveArea(new RangeFloat2(5f, 20f), new RangeFloat2(-30f, 30f), new RangeFloat2(-10f, 10f));
	[Range(0, 1)]
	public float acceleration = 0.5f;
	[Range(0, 1)]
	public float agility = 0.5f;
	public MoveAdjust moveAdjust = new MoveAdjust(0.5f, 0.5f);

	[Header("Attack Properties")]
	public RangeFloat4 attackDistance = new RangeFloat4(0f, 0f, 10f, 30f);
	public RangeFloat2 attackDistanceLoseBySteer = new RangeFloat2(0f, 0.5f);
	public float attackMaxAngle = 2.5f;
	public float attackPower = 5f;
	public float attackRolls = 10f;
	public float attackPowerGainedPerSpeed = 0.1f;
	[Range(0, 1)]
	public float attackBaseProbability = 0.8f;
}
