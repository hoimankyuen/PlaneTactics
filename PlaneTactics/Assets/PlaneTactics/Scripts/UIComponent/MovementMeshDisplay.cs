using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMeshDisplay : MeshDisplay
{
	static readonly int distanceSegments = 16;
	static readonly int angleSegments = 16;

	protected float height;
	protected MoveArea moveArea;

	public void Modify(float height, MoveArea moveArea)
	{
		if (this.height != height || this.moveArea != moveArea)
		{
			this.height = height;
			this.moveArea = moveArea;
			RecreateMesh(mesh, height, moveArea);
		}
	}

	public void RecreateMesh(Mesh mesh, float height, MoveArea moveArea)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector4> tangents = new List<Vector4>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		for (int i = 0; i < angleSegments + 1; i++)
		{
			for (int j = 0; j < distanceSegments + 1; j++)
			{
				MoveInput input = new MoveInput((float)j / distanceSegments, (float)i / angleSegments);
				MoveResult result = moveArea.Evaluate(input);

				vertices.Add(
						Vector3.forward * result.distance * Mathf.Cos(result.azimuth * Mathf.Deg2Rad) +
						Vector3.right * result.distance * Mathf.Sin(result.azimuth * Mathf.Deg2Rad) +
						Vector3.up * height);
				normals.Add(Vector3.up);
				tangents.Add(new Vector4(1, 0, 0, -1));
				uv.Add(new Vector2((float)i / angleSegments, (float)j / distanceSegments));
				if (i > 0 && j > 0)
				{
					triangles.AddRange(new int[] {
						(i - 1) * (distanceSegments + 1) + (j - 1),
						(i - 1) * (distanceSegments + 1) + (j - 0),
						(i - 0) * (distanceSegments + 1) + (j - 1),
						(i - 0) * (distanceSegments + 1) + (j - 1),
						(i - 1) * (distanceSegments + 1) + (j - 0),
						(i - 0) * (distanceSegments + 1) + (j - 0)
					});
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

