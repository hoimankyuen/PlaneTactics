using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightMeshDisplay : MeshDisplay
{
	static readonly float bodySize = 0.25f;
	static readonly int bodySegments = 8;
	static readonly float headRadius = 1.5f;
	static readonly int headSegments = 8;

	protected float height;
	protected float heightPerLevel;

	// ========================================================= Specific Functionalities =========================================================

	/// <summary>
	/// Change the shape of the generated mesh according to the input parameters.
	/// </summary>
	public void Modify(float height, float heightPerLevel)
	{
		if (this.height != height || this.heightPerLevel != heightPerLevel)
		{
			this.height = height;
			this.heightPerLevel = heightPerLevel;
			RecreateMesh(mesh, height, heightPerLevel);
		}
	}

	/// <summary>
	/// Recrate the generated mesh according to the input parameters.
	/// </summary>
	protected void RecreateMesh(Mesh mesh, float height, float heightPerLevel)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector4> tangents = new List<Vector4>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < bodySegments; k++)
			{
				Vector3 normalPoint = new Vector3(
					Mathf.Cos(((float)k / bodySegments + 0.25f) * Mathf.PI * 2),
					0,
					Mathf.Sin(((float)k / bodySegments + 0.25f) * Mathf.PI * 2));
				vertices.Add(normalPoint * (j > 0 && j < 3 ? bodySize : 0) + Vector3.up * Mathf.Lerp(0, height, Mathf.InverseLerp(1, 2, j)));
				normals.Add(j > 0 && j < 3 ? normalPoint : j == 0 ? Vector3.down : Vector3.up);
				uv.Add(new Vector2((float)k / (bodySegments - 1) / 2, Mathf.Lerp(0, height, Mathf.InverseLerp(1, 2, j)) / heightPerLevel));
				tangents.Add(new Vector4(0, 0, 0, -1) + (Vector4)normalPoint);
				if (j > 0)
				{
					triangles.AddRange(new int[] {
						(j - 1) * bodySegments + (k + 0) % bodySegments,
						(j - 0) * bodySegments + (k + 0) % bodySegments,
						(j - 1) * bodySegments + (k + 1) % bodySegments,
						(j - 1) * bodySegments + (k + 1) % bodySegments,
						(j - 0) * bodySegments + (k + 0) % bodySegments,
						(j - 0) * bodySegments + (k + 1) % bodySegments
					});
				}
			}
		}

		for (int i = 1; i < (height - headRadius) / heightPerLevel; i++)
		{

			for (int j = 0; j < headSegments / 2 + 1; j++)
			{
				for (int k = 0; k < headSegments; k++)
				{
					Vector3 normalPoint = new Vector3(
										Mathf.Cos(((float)j / headSegments * 2) * Mathf.PI - Mathf.PI * 0.5f) * Mathf.Cos(((float)k / headSegments + 0.5f) * Mathf.PI * 2),
										Mathf.Sin(((float)j / headSegments * 2) * Mathf.PI - Mathf.PI * 0.5f),
										Mathf.Cos(((float)j / headSegments * 2) * Mathf.PI - Mathf.PI * 0.5f) * Mathf.Sin(((float)k / headSegments + 0.5f) * Mathf.PI * 2));
					vertices.Add(normalPoint * headRadius + Vector3.up * i * heightPerLevel);
					normals.Add(normalPoint);
					tangents.Add(new Vector4(0, 0, 0, -1) + (Vector4)normalPoint);
					uv.Add(new Vector2((float)k / (headSegments - 1) / 2 + 0.5f, (float)j / (headSegments / 2)));
					if (j > 0)
					{
						int offset = bodySegments * 4 + (i - 1) * headSegments * (headSegments / 2 + 1);
						triangles.AddRange(new int[] {
							offset + (j - 1) * headSegments + (k + 0) % headSegments,
							offset + (j - 0) * headSegments + (k + 0) % headSegments,
							offset + (j - 1) * headSegments + (k + 1) % headSegments,
							offset + (j - 1) * headSegments + (k + 1) % headSegments,
							offset + (j - 0) * headSegments + (k + 0) % headSegments,
							offset + (j - 0) * headSegments + (k + 1) % headSegments
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

