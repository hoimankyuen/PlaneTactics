using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTeam", menuName = "Plane Fight/Team Info", order = 3)]
public class TeamInfo : ScriptableObject
{
	[Header("Basic Information")]
	public new string name = "New Team";
	public Sprite insignia = null;
	public Sprite finFlash = null;
	public Color color = Color.white;

	[Header("Other Properties")]
	public int priority = 0;
}
