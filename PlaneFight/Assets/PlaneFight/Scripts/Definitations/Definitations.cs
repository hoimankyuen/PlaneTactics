using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitClass
{
	Biplane,
	LightFighter,
	HeavyFighter,
	SuperFighter,
	JetFighter,
	Bomber,
	Reconnaissance,
	Rotorcraft,
	Command,
	Transport
}

public enum Buff
{
	None,
	Defence,
	Speed,
	Turn,
	Attack,
	Range,
	Accuracy,
	View
}

public enum BuffMagnitude
{
	None,
	Buff1,
	Buff2,
	Nerf1,
	Nerf2,
	Disable
}