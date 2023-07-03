using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMeshDisplay : MeshDisplay
{

	static readonly float width = 5f;
	static readonly float headLength = 10f;
	static readonly int headSegments = 4;
	static readonly int tailSegments = 12;

	protected float height;
	protected IPath turnCurve;

	// ========================================================= Specific Functionalities =========================================================

	/// <summary>
	/// Change the shape of the generated mesh according to the input parameters.
	/// </summary>
	public void Modify(float height, IPath turnCurve)
	{
		if (this.height != height || this.turnCurve != turnCurve)
		{
			this.height = height;
			this.turnCurve = turnCurve;
			RecreateMesh(mesh, height, turnCurve);
		}
	}

	/// <summary>
	/// Recrate the generated mesh according to the input parameters.
	/// </summary>
	protected void RecreateMesh(Mesh mesh, float height, IPath path)
	{
		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector4> tangents = new List<Vector4>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		float tailPortion = 1 - path.ValueByLength(headLength);

		for (int i = 0; i < tailSegments + 1; i++)
		{
			float point = Mathf.Lerp(0, tailPortion, (float)i / tailSegments);

			vertices.Add(path.PointAt(point) - Vector3.Cross(path.DirectionProjectionAt(point), path.UpAt(point)) * width / 2f + Vector3.up * height);
			vertices.Add(path.PointAt(point) + Vector3.Cross(path.DirectionProjectionAt(point), path.UpAt(point)) * width / 2f + Vector3.up * height);
			normals.Add(path.UpAt(point));
			normals.Add(path.UpAt(point));
			tangents.Add(new Vector4(1, 0, 0, -1));
			tangents.Add(new Vector4(1, 0, 0, -1));
			uv.Add(new Vector2(0f, 0.5f * i / tailSegments));
			uv.Add(new Vector2(1f, 0.5f * i / tailSegments));
			if (i > 0)
			{
				triangles.AddRange(new int[] { i * 2 - 2, i * 2 - 1, i * 2 - 0, i * 2 - 0, i * 2 - 1, i * 2 + 1 });
			}
		}
		int headIndex = (tailSegments + 1) * 2;
		Vector3 right = Vector3.Cross(path.DirectionProjectionAt(tailPortion), Vector3.up);
		for (int i = 0; i < headSegments + 1; i++)
		{
			float point = Mathf.Lerp(tailPortion, 1, (float)i / headSegments);
			vertices.AddRange(new Vector3[] {
				path.PointAt(point) - right * width * (1 - (float)i / headSegments) + Vector3.up * height,
				path.PointAt(point) - right * width * (1 - (float)i / headSegments) / 2f + Vector3.up * height,
				path.PointAt(point) + right * width * (1 - (float)i / headSegments) / 2f + Vector3.up * height,
				path.PointAt(point) + right * width * (1 - (float)i / headSegments) + Vector3.up * height
			});
			normals.AddRange(new Vector3[] {
				path.UpAt(point),
				path.UpAt(point),
				path.UpAt(point),
				path.UpAt(point)
			});
			tangents.AddRange(new Vector4[] {
				new Vector4(1, 0, 0, -1),
				new Vector4(1, 0, 0, -1),
				new Vector4(1, 0, 0, -1),
				new Vector4(1, 0, 0, -1)
			});
			uv.AddRange(new Vector2[] {
				new Vector2(Mathf.Lerp(1f, 0.5f, (float) i / headSegments), 1f),
				new Vector2(Mathf.Lerp(1f, 0.5f, (float) i / headSegments), Mathf.Lerp(0.5f, 1f, (float) i / headSegments)),
				new Vector2(Mathf.Lerp(0f, 0.5f, (float) i / headSegments), Mathf.Lerp(0.5f, 1f, (float) i / headSegments)),
				new Vector2(Mathf.Lerp(0f, 0.5f, (float) i / headSegments), 1f)
			});
			if (i > 0)
			{
				triangles.AddRange(new int[] {
					headIndex + i * 4 - 4, headIndex + i * 4 - 3, headIndex + i * 4 + 0, headIndex + i * 4 + 0, headIndex + i * 4 - 3, headIndex + i * 4 + 1,
					headIndex + i * 4 - 3, headIndex + i * 4 - 2, headIndex + i * 4 + 1, headIndex + i * 4 + 1, headIndex + i * 4 - 2, headIndex + i * 4 + 2,
					headIndex + i * 4 - 2, headIndex + i * 4 - 1, headIndex + i * 4 + 2, headIndex + i * 4 + 2, headIndex + i * 4 - 1, headIndex + i * 4 + 3,
				});
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