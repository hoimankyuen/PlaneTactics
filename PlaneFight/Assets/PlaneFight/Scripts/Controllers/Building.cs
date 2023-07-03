using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

	[Header("Information")]
	public float captureRange = 50f;
	public TeamInfo teamInfo = null;

	// references
	protected LevelController level = null;
	protected new CameraController camera = null;
	protected PlayerInput playerInput = null;

	// ========================================================= Monobehaviour Methods =========================================================

	/// <summary>
	/// Awake is called when the game object was created. It is always called before start and is 
	/// independent of if the game object is active or not.
	/// </summary>
	protected void Awake()
	{
		RetrieveReferences();
		RegisterToComponents();
	}

	/// <summary>
	/// Start is called before the first frame update and/or the game object is first active.
	/// </summary>
	void Start()
    {
        
    }

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	void Update()
    {
        
    }

	// ========================================================= Initiation and Destruction =========================================================

	/// <summary>
	/// Retrieve all required references.
	/// </summary>
	protected void RetrieveReferences()
	{
		// retrieve camera control
		level = FindObjectOfType<LevelController>();
		camera = FindObjectOfType<CameraController>();
		playerInput = FindObjectOfType<PlayerInput>();
	}

	/// <summary>
	/// Register this unit to higher level components.
	/// </summary>
	protected void RegisterToComponents()
	{
		level.RegisterBuilding(this);
	}

	/// <summary>
	/// Deregister this unit from higher level components.
	/// </summary>
	protected void DeregisterFromComponents()
	{
		level.DeregisterBuilding(this);
	}
}
