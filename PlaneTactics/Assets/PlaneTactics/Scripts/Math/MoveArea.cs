using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MoveArea
{
	public RangeFloat2 distances;
	public RangeFloat2 nearAngles;
	public RangeFloat2 farAngles;

	public static bool operator ==(MoveArea a, MoveArea b) => a.Equals(b);

	public static bool operator !=(MoveArea a, MoveArea b) => !a.Equals(b);

	public MoveArea(RangeFloat2 distances, RangeFloat2 nearAngles, RangeFloat2 farAngles)
	{
		this.distances = distances;
		this.nearAngles = nearAngles;
		this.farAngles = farAngles;
	}

	public MoveArea Expand(RangeFloat2 distances, RangeFloat2 nearAngles, RangeFloat2 farAngles)
	{
		return new MoveArea(
			this.distances + distances,
			this.nearAngles + nearAngles,
			this.farAngles + farAngles);
	}

	public MoveArea SubArea(MoveInput input, MoveAdjust adjust)
	{
		RangeFloat2 nearAnglesBySpeed = new RangeFloat2(
			Mathf.Lerp(nearAngles.min, farAngles.min, input.speed - adjust.acceleration),
			Mathf.Lerp(nearAngles.max, farAngles.max, input.speed - adjust.acceleration));
		RangeFloat2 farAnglesBySpeed = new RangeFloat2(
			Mathf.Lerp(nearAngles.min, farAngles.min, input.speed + adjust.acceleration),
			Mathf.Lerp(nearAngles.max, farAngles.max, input.speed + adjust.acceleration));
		return new MoveArea(
			new RangeFloat2(
				distances.Lerp(input.speed - adjust.acceleration),
				distances.Lerp(input.speed + adjust.acceleration)),
			new RangeFloat2(
				nearAnglesBySpeed.Lerp(input.steer - adjust.agility),
				nearAnglesBySpeed.Lerp(input.steer + adjust.agility)),
			new RangeFloat2(
				farAnglesBySpeed.Lerp(input.steer - adjust.agility),
				farAnglesBySpeed.Lerp(input.steer + adjust.agility)));
	}

	public bool IsInclude(MoveResult result)
	{
		float speed = distances.InverseLerp(result.distance);
		RangeFloat2 angles = new RangeFloat2(
			Mathf.Lerp(nearAngles.min, farAngles.min, speed),
			Mathf.Lerp(nearAngles.max, farAngles.max, speed));
		return distances.IsInclude(result.distance) && angles.IsInclude(result.azimuth);
	}

	public MoveResult Clamp(MoveResult result)
	{
		float speed = distances.InverseLerp(result.distance);
		RangeFloat2 angles = new RangeFloat2(
			Mathf.Lerp(nearAngles.min, farAngles.min, speed),
			Mathf.Lerp(nearAngles.max, farAngles.max, speed));
		return new MoveResult(
			distances.Clamp(result.distance),
			angles.Clamp(result.azimuth));
	}

	public MoveResult Evaluate(MoveInput input)
	{
		RangeFloat2 angles = new RangeFloat2(
			Mathf.Lerp(nearAngles.min, farAngles.min, input.speed),
			Mathf.Lerp(nearAngles.max, farAngles.max, input.speed));
		return new MoveResult(
			distances.Lerp(input.speed),
			angles.Lerp(input.steer));
	}

	public MoveInput Inverse(MoveResult result)
	{
		float speed = distances.InverseLerp(result.distance);
		RangeFloat2 angles = new RangeFloat2(
			Mathf.Lerp(nearAngles.min, farAngles.min, speed),
			Mathf.Lerp(nearAngles.max, farAngles.max, speed));
		return new MoveInput(
			speed,
			angles.InverseLerp(result.azimuth));
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MoveArea other))
			return false;

		return distances == other.distances && nearAngles == other.nearAngles && farAngles == other.farAngles;
	}

	public override int GetHashCode()
	{
		var hashCode = -897720056;
		hashCode = hashCode * -1521134295 + distances.GetHashCode();
		hashCode = hashCode * -1521134295 + nearAngles.GetHashCode();
		hashCode = hashCode * -1521134295 + farAngles.GetHashCode();
		return hashCode;
	}
}

[System.Serializable]
public struct MoveInput
{
	public float speed;
	public float steer;

	public MoveInput(float speed, float steer)
	{
		this.speed = speed;
		this.steer = steer;
	}
}

[System.Serializable]
public struct MoveAdjust
{
	public float acceleration;
	public float agility;

	public MoveAdjust(float acceleration, float agility)
	{
		this.acceleration = acceleration;
		this.agility = agility;
	}
}

[System.Serializable]
public struct MoveResult
{
	public float distance;
	public float azimuth;

	public MoveResult(float distance, float azimuth)
	{
		this.distance = distance;
		this.azimuth = azimuth;
	}
}
