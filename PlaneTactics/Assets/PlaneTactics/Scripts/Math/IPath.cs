using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPath
{
	// ========================================================= Position =========================================================
	
	Vector3 PointProjectionAt(float t);

	float HeightAt(float t);

	Vector3 PointAt(float t);

	// ========================================================= Direction =========================================================
	
	Vector3 DirectionProjectionAt(float t);

	Vector3 UpAt(float t);

	Vector3 DirectionAt(float t);

	// ========================================================= Evaluation =========================================================
	
	float ValueByLength(float length);
}





public struct DropTrunPath : IPath
{
	public SimpleTurnPath turnPath;
	public float gravity;
	public float duration;

	public DropTrunPath(SimpleTurnPath turnPath, float gravity, float duration)
	{
		this.turnPath = turnPath;
		this.gravity = gravity;
		this.duration = duration;
	}


	// ========================================================= Position =========================================================
	
	public Vector3 PointProjectionAt(float t)
	{
		return turnPath.PointAt(duration * t);
	}

	public float HeightAt(float t)
	{
		return 0.5f * gravity * duration * t * duration * t;
	}

	public Vector3 PointAt(float t)
	{
		return PointProjectionAt(t) + Vector3.up * HeightAt(t);
	}

	// ========================================================= Direction =========================================================

	public Vector3 DirectionProjectionAt(float t)
	{
		return turnPath.DirectionProjectionAt(duration * t);
	}

	public Vector3 UpAt(float t)
	{
		return Vector3.up;
	}

	public Vector3 DirectionAt(float t)
	{
		return turnPath.DirectionAt(duration * t);
	}

	// ========================================================= Evaluation =========================================================

	public float ValueByLength(float length)
	{
		return turnPath.ValueByLength(length / duration);
	}
}





public struct BlankPath : IPath
{
	public Vector3 PointProjectionAt(float t)
	{
		return Vector3.zero;
	}

	public float HeightAt(float t)
	{
		return 0f;
	}

	public Vector3 PointAt(float t)
	{
		return Vector3.zero;
	}

	public Vector3 DirectionAt(float t)
	{
		return Vector3.forward;
	}

	public Vector3 DirectionProjectionAt(float t)
	{
		return Vector3.forward;
	}

	public Vector3 UpAt(float t)
	{
		return Vector3.up;
	}

	public float ValueByLength(float length)
	{
		return 0;
	}
}