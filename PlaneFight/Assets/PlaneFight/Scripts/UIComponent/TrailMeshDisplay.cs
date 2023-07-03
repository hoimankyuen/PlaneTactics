using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailMeshDisplay : MeshDisplay
{
	static readonly float size = 0.25f;
	static readonly int segments = 16;

	protected float height;
	protected List<IPath> paths;
	protected List<float> tilts;
	protected Vector3 offset;
	protected float width;
	protected float t;

	// ========================================================= Monobehaviour Methods =========================================================

	/// <summary>
	/// Awake is called when the game object was created. It is always called before start and is 
	/// independent of if the game object is active or not.
	/// </summary>
	protected new void Awake()
	{
		base.Awake();
		Transform offsetTransform = transform.Find("Offset");
		offset = offsetTransform.localPosition;
		width = offsetTransform.localScale.x;
		offsetTransform.gameObject.SetActive(false);
	}

	// ========================================================= Specific Functionalities =========================================================

	/// <summary>
	/// Change the shape of the generated mesh according to the input parameters.
	/// </summary>
	public void Modify(float height, List<IPath> paths, List<float> tilts, float t)
	{
		if (this.height != height || this.t != t)
		{
			bool identical = true;
			if (this.paths == null || this.tilts == null || this.paths.Count != paths.Count || this.tilts.Count != tilts.Count)
			{
				identical = false;
			}

			if (identical)
			{
				for (int i = 0; i < paths.Count; i++)
				{
					if (this.paths[i] != paths[i])
					{
						identical = false;
						break;
					}
				}
			}

			if (identical)
			{
				for (int i = 0; i < tilts.Count; i++)
				{
					if (this.tilts[i] != tilts[i])
					{
						identical = false;
						break;
					}
				}
			}

			if (!identical)
			{
				RecreateMesh(mesh, height, paths, tilts, offset, width, t);
			}
		}
	}

	/// <summary>
	/// Recrate the generated mesh according to the input parameters.
	/// </summary>
	public void RecreateMesh(Mesh mesh, float height, List<IPath> paths, List<float> tilts, Vector3 offset, float width, float t)
	{
		t = Mathf.Clamp01(t);

		List<Vector3> vertices = new List<Vector3>();
		List<Vector3> normals = new List<Vector3>();
		List<Vector4> tangents = new List<Vector4>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		for (int i = 0; i < (paths.Count - 1) * segments + 1; i++)
		{
			// calculate vertex relationship with path
			int curveIndex = (int)(t + (float)i / segments) - ((t == 1 && i == (paths.Count - 1) * segments) ? 1 : 0);
			float point = (t + (float)i / segments) % 1 + ((t == 1 && i == (paths.Count - 1) * segments) ? 1 : 0);

			// calculate accumulative offsets from latest to earliest path
			Quaternion rotationOffset = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, paths[paths.Count - 1].DirectionProjectionAt(t)));
			Vector3 positionOffset = Vector3.zero - rotationOffset * paths[paths.Count - 1].PointAt(t);
			Vector3 heightOffset = Vector3.up * height - Vector3.up * paths[paths.Count - 1].HeightAt(1 - t);
			for (int j = paths.Count - 2; j >= curveIndex; j--)
			{
				rotationOffset = Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, paths[j].DirectionProjectionAt(1))) * rotationOffset;
				positionOffset = positionOffset - rotationOffset * paths[j].PointAt(1);
			}

			// construct vectors
			Vector3 pos = paths[curveIndex].PointAt(point);
			Vector3 forward = paths[curveIndex].DirectionAt(point);
			Vector3 right = Quaternion.AngleAxis(Mathf.SmoothStep(tilts[curveIndex], tilts[curveIndex + 1], point), paths[curveIndex].DirectionProjectionAt(point)) *
				Vector3.Cross(paths[curveIndex].DirectionAt(point), paths[curveIndex].UpAt(point));
			Vector3 up = Vector3.Cross(right, forward);

			// apply trail offset 
			pos = pos + right * offset.x + up * offset.y + forward * offset.z;

			vertices.AddRange(new Vector3[] {
						positionOffset + rotationOffset * (pos - right * (width / 2f + size / 2f) + up * size / 2f) + heightOffset,
						positionOffset + rotationOffset * (pos - right * (width / 2f - size / 2f) + up * size / 2f) + heightOffset,
						positionOffset + rotationOffset * (pos - right * (width / 2f - size / 2f) - up * size / 2f) + heightOffset,
						positionOffset + rotationOffset * (pos - right * (width / 2f + size / 2f) - up * size / 2f) + heightOffset,

						positionOffset + rotationOffset * (pos + right * (width / 2f - size / 2f) + up * size / 2f) + heightOffset,
						positionOffset + rotationOffset * (pos + right * (width / 2f + size / 2f) + up * size / 2f) + heightOffset,
						positionOffset + rotationOffset * (pos + right * (width / 2f + size / 2f) - up * size / 2f) + heightOffset,
						positionOffset + rotationOffset * (pos + right * (width / 2f - size / 2f) - up * size / 2f) + heightOffset,

						positionOffset + rotationOffset * (pos - right * (width / 2f + size * 4f)) + heightOffset,
						positionOffset + rotationOffset * (pos + right * (width / 2f + size * 4f)) + heightOffset
			});

			normals.AddRange(new Vector3[] {
						(-right + up).normalized,
						( right + up).normalized,
						( right - up).normalized,
						(-right - up).normalized,

						(-right + up).normalized,
						( right + up).normalized,
						( right - up).normalized,
						(-right - up).normalized,

						-right,
						right
			});

			tangents.AddRange(new Vector4[] {
						 new Vector4(0, 0, 0, -1) + (Vector4)(-right + up).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)( right + up).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)( right - up).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)(-right - up).normalized,

						 new Vector4(0, 0, 0, -1) + (Vector4)(-right + up).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)( right + up).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)( right - up).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)(-right - up).normalized,

						 new Vector4(0, 0, 0, -1) + (Vector4)(-right).normalized,
						 new Vector4(0, 0, 0, -1) + (Vector4)( right).normalized
			});

			uv.AddRange(new Vector2[] {
						new Vector2(0.01f, (float)i / ((paths.Count - 1) * segments)),
						new Vector2(0.49f, (float)i / ((paths.Count - 1) * segments)),
						new Vector2(0.01f, (float)i / ((paths.Count - 1) * segments)),
						new Vector2(0.49f, (float)i / ((paths.Count - 1) * segments)),

						new Vector2(0.01f, (float)i / ((paths.Count - 1) * segments)),
						new Vector2(0.49f, (float)i / ((paths.Count - 1) * segments)),
						new Vector2(0.01f, (float)i / ((paths.Count - 1) * segments)),
						new Vector2(0.49f, (float)i / ((paths.Count - 1) * segments)),

						new Vector2(0.74f, (1 - point + 0.25f) % 1f - 0.25f),
						new Vector2(0.76f, (1 - point + 0.25f) % 1f - 0.25f)
			});

			if (i > 0)
			{
				triangles.AddRange(new int[] {
							i * 10 - 10, i * 10 -  9, i * 10 +  0, i * 10 +  0, i * 10 -  9, i * 10 +  1,
							i * 10 -  9, i * 10 -  8, i * 10 +  1, i * 10 +  1, i * 10 -  8, i * 10 +  2,
							i * 10 -  8, i * 10 -  7, i * 10 +  2, i * 10 +  2, i * 10 -  7, i * 10 +  3,
							i * 10 -  7, i * 10 - 10, i * 10 +  3, i * 10 +  3, i * 10 - 10, i * 10 +  0,

							i * 10 -  6, i * 10 -  5, i * 10 +  4, i * 10 +  4, i * 10 -  5, i * 10 +  5,
							i * 10 -  5, i * 10 -  4, i * 10 +  5, i * 10 +  5, i * 10 -  4, i * 10 +  6,
							i * 10 -  4, i * 10 -  3, i * 10 +  6, i * 10 +  6, i * 10 -  3, i * 10 +  7,
							i * 10 -  3, i * 10 -  6, i * 10 +  7, i * 10 +  7, i * 10 -  6, i * 10 +  4,

							i * 10 -  2, i * 10 -   1, i * 10 +  8, i * 10 +  8, i * 10 -  1, i * 10 +  9,
							i * 10 -  1, i * 10 -   2, i * 10 +  9, i * 10 +  9, i * 10 -  2, i * 10 +  8,
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
