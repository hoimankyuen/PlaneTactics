using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMeshDisplay : MeshDisplay
{
	static readonly int segments = 4;

	protected float height;
	protected RangeFloat4 distances;
	protected float maxAngle;

	// ========================================================= Specific Functionalities =========================================================

	/// <summary>
	/// Change the shape of the generated mesh according to the input parameters.
	/// </summary>
	public void Modify(float height, RangeFloat4 distances, float maxAngle)
	{
		if (this.height != height || this.distances != distances || this.maxAngle != maxAngle)
		{
			this.height = height;
			this.distances = distances;
			this.maxAngle = maxAngle;
			RecreateMesh(mesh, height, distances, maxAngle);
		}
	}

	/// <summary>
	/// Recrate the generated mesh according to the input parameters.
	/// </summary>
	protected void RecreateMesh(Mesh mesh, float height, RangeFloat4 distances, float maxAngle)
	{
		float[] partDistances = new float[] { distances.min, distances.midl, distances.midg, distances.max };

		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector4> tangents = new List<Vector4>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		for (int i = 0; i < segments + 1; i++)
		{
			float angle = Mathf.Lerp(-maxAngle, maxAngle, (float)i / segments);
			for (int j = 0; j < 4; j++)
			{
				vertices.Add(
						Vector3.forward * partDistances[j] * Mathf.Cos(angle * Mathf.Deg2Rad) +
						Vector3.right * partDistances[j] * Mathf.Sin(angle * Mathf.Deg2Rad) +
						Vector3.up * height);
				normals.Add(Vector3.up);
				tangents.Add(new Vector4(1, 0, 0, -1));
				uv.Add(new Vector2((float)i / segments, (float)j / 3));
				if (i > 0 && j > 0)
				{
					if (i > segments / 2)
					{
						triangles.AddRange(new int[] {
							(i - 1) * 4 + (j - 1),
							(i - 1) * 4 + (j - 0),
							(i - 0) * 4 + (j - 1),
							(i - 0) * 4 + (j - 1),
							(i - 1) * 4 + (j - 0),
							(i - 0) * 4 + (j - 0)
						});
					}
					else
					{
						triangles.AddRange(new int[] {
							(i - 0) * 4 + (j - 1),
							(i - 1) * 4 + (j - 1),
							(i - 0) * 4 + (j - 0),
							(i - 0) * 4 + (j - 0),
							(i - 1) * 4 + (j - 1),
							(i - 1) * 4 + (j - 0)
						});
					}
				}
			}
		}

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.normals = normals.ToArray();
		mesh.tangents = tangents.ToArray();
		mesh.uv = uv.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateBounds();
	}
}

