using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPilot", menuName = "Plane Fight/Pilot Info", order = 2)]
public class PilotInfo : ScriptableObject
{
	[Header("Basic Information")]
	public new string name = "New Pilot";
}
