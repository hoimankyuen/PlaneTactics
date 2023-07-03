using System.Collections.Generic;
using UnityEngine;

public class MeshDisplay : MonoBehaviour
{
	protected new MeshRenderer renderer = null;
	protected Material material = null;
	protected Mesh mesh = null;

	// ========================================================= Monobehaviour Methods =========================================================

	/// <summary>
	/// Awake is called when the game object was created. It is always called before start and is 
	/// independent of if the game object is active or not.
	/// </summary>
	protected void Awake()
	{
		mesh = new Mesh
		{
			name = name + "Mesh",
		};
		GetComponent<MeshFilter>().mesh = mesh;
		renderer = GetComponent<MeshRenderer>();
		material = Material.Instantiate(renderer.material);
		renderer.material = material;
	}

	/// <summary>
	/// Destroy is called when the game object is destroyed.
	/// </summary>
	protected void OnDestroy()
	{
		Mesh.Destroy(mesh);
		Material.Destroy(material);
	}

	// ========================================================= Functionalities =========================================================

	/// <summary>
	/// Show the generated model accociated with this mesh display.
	/// </summary>
	public void Show(bool show)
	{
		renderer.enabled = show;
	}

	/// <summary>
	/// Change the color of the generated model accociated with this mesh display.
	/// </summary>
	public void SetColor(Color color)
	{
		material.color = color;
	}
}

