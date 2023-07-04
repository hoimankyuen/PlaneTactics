using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SimpleTurnPath : IPath
{
	private static SimpleTurnPath pastTurnPath;
	private static TurnPathResults turnPathResults;

	private struct TurnPathResults
	{
		public float radius;
		public float arcAngle;
		public TurnPathResults(float radius, float arcAngle)
		{
			this.radius = radius;
			this.arcAngle = arcAngle;
		}
	}

	public float distance;
	public float azimuth;
	public float climb;

	public static bool operator ==(SimpleTurnPath a, SimpleTurnPath b) => a.Equals(b);

	public static bool operator !=(SimpleTurnPath a, SimpleTurnPath b) => !a.Equals(b);

	public SimpleTurnPath(float distance, float azimuth, float climb)
	{
		this.distance = distance;
		this.azimuth = azimuth;
		this.climb = climb;
	}

	// ========================================================= Position =========================================================
	
	public Vector3 PointProjectionAt(float t)
	{
		if (pastTurnPath.distance != distance || pastTurnPath.azimuth != azimuth)
		{
			Recalculate();
		}
		if (azimuth != 0)
		{
			return new Vector3(
				Mathf.Cos(Mathf.PI - turnPathResults.arcAngle * t) * turnPathResults.radius + turnPathResults.radius,
				0,
				Mathf.Sin(Mathf.PI - turnPathResults.arcAngle * t) * turnPathResults.radius);
		}
		else
		{
			return new Vector3(0, 0, distance * t);
		}
	}

	public float HeightAt(float t)
	{
		if (pastTurnPath.distance != distance || pastTurnPath.azimuth != azimuth)
		{
			Recalculate();
		}
		return Mathf.SmoothStep(0, climb, t);
	}

	public Vector3 PointAt(float t)
	{
		return PointProjectionAt(t) + Vector3.up * HeightAt(t);
	}

	// ========================================================= Direction =========================================================
	
	public Vector3 DirectionProjectionAt(float t)
	{
		if (pastTurnPath.distance != distance || pastTurnPath.azimuth != azimuth)
		{
			Recalculate();
		}
		if (azimuth != 0)
		{
			return new Vector3(
				Mathf.Cos(Mathf.PI / 2 - turnPathResults.arcAngle * t),
				0,
				Mathf.Sin(Mathf.PI / 2 - turnPathResults.arcAngle * t));
		}
		else
		{
			return Vector3.forward;
		}
	}

	public Vector3 UpAt(float t)
	{
		return Vector3.Cross(Vector3.Cross(DirectionProjectionAt(t), Vector3.up), DirectionAt(t)).normalized;
	}

	public Vector3 DirectionAt(float t)
	{
		if (pastTurnPath.distance != distance || pastTurnPath.azimuth != azimuth)
		{
			Recalculate();
		}
		if (azimuth != 0)
		{
			return new Vector3(
				Mathf.Cos(Mathf.PI / 2 - turnPathResults.arcAngle * t) * turnPathResults.arcAngle * turnPathResults.radius,
				(-6f * t * t + 6f * t) * climb,
				Mathf.Sin(Mathf.PI / 2 - turnPathResults.arcAngle * t) * turnPathResults.arcAngle * turnPathResults.radius).normalized;
		}
		else
		{
			return new Vector3(
				0f,
				(-6f * t * t + 6f * t) * climb,
				distance).normalized;
		}
	}

	// ========================================================= Evaluation =========================================================
	
	public float ValueByLength(float length)
	{
		if (pastTurnPath.distance != distance || pastTurnPath.azimuth != azimuth)
		{
			Recalculate();
		}
		if (azimuth != 0)
		{
			return length / turnPathResults.arcAngle / turnPathResults.radius;
		}
		else
		{
			return length / distance;
		}
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SimpleTurnPath other))
			return false;

		return distance == other.distance && azimuth == other.azimuth && climb == other.climb;
	}

	public override int GetHashCode()
	{
		var hashCode = -897720056;
		hashCode = hashCode * -1521134295 + distance.GetHashCode();
		hashCode = hashCode * -1521134295 + azimuth.GetHashCode();
		hashCode = hashCode * -1521134295 + climb.GetHashCode();
		return hashCode;
	}

	private void Recalculate()
	{
		if (azimuth != 0)
		{
			Vector3 endPoint = new Vector3(Mathf.Sin(azimuth * Mathf.Deg2Rad) * distance, 0, Mathf.Cos(azimuth * Mathf.Deg2Rad) * distance);
			turnPathResults.radius = (endPoint.x * endPoint.x + endPoint.z * endPoint.z) / 2f / endPoint.x;
			if (turnPathResults.radius > 0)
			{
				turnPathResults.arcAngle = Mathf.PI - Vector3.SignedAngle(Vector3.right, endPoint - new Vector3(turnPathResults.radius, 0, 0), Vector3.up * -1) * Mathf.Deg2Rad;
			}
			else
			{
				turnPathResults.arcAngle = (Mathf.PI - Vector3.SignedAngle(Vector3.right * -1, endPoint - new Vector3(turnPathResults.radius, 0, 0), Vector3.up) * Mathf.Deg2Rad) * -1;
			}
		}
		else
		{
			turnPathResults.radius = Mathf.Infinity;
			turnPathResults.arcAngle = 0;
		}

		pastTurnPath.distance = distance;
		pastTurnPath.azimuth = azimuth;
	}
}