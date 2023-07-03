using QuickerEffects;
using SimpleMaskCutoff;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerUnit : MonoBehaviour
{
	
	protected enum UnitMarkerType
	{
		None,
		Normal,
		Projection,
		Damage,
		Collision,
	}

	protected struct AttackInfo
	{
		public PlayerUnit target;
		public float probability;
	}

	protected struct DamageInfo
	{
		public PlayerUnit source;
		public List<float> hits;
	}

	// information
	[Header("Information")]
	public UnitInfo unitInfo = null;
	public PilotInfo pilotInfo = null;
	public TeamInfo teamInfo = null;

	[Header("Initial Settings")]
	[Range(0, 6)]
	public int initialAltitude = 2;
	[Range(0, 1)]
	public float initialHealthPercentage = 1f;
	[Range(0, 1)]
	public float initialSpeed = 0.5f;
	[Range(0, 1)]
	public float initialSteer = 0.5f;

	// references
	protected LevelController level = null;
	protected new CameraController camera = null;
	protected PlayerInput playerInput = null;

	// working plane info copy
	protected float maxHealth = 100f;
	protected float hitboxRadius = 1.5f;
	protected MoveArea moveArea;
	protected MoveAdjust moveAdjust;
	protected RangeFloat4 attackDistances;
	protected RangeFloat2 attackDistanceLoseBySteer;
	protected float attackMaxAngle = 5f;
	protected float attackPower = 5f;
	protected float attackRolls = 10f;
	protected float attackBaseProbability = 0.8f;

	// working variables
	protected Vector3 preActionPosition = Vector3.zero;
	protected Vector3 postActionPosition = Vector3.zero;
	protected Quaternion preActionRotation = Quaternion.identity;
	protected Quaternion postActionRotation = Quaternion.identity;
	protected int preActionAltitude = 0;
	protected int postActionAltitude = 0;
	protected float preActionHealth = 0;
	protected float postActionHealth = 0;
	protected float preActionSpeed = 0;
	protected float postActionSpeed = 0;
	protected float preActionSteer = 0;
	protected float postActionSteer = 0;
	protected SimpleTurnPath preActionMovePath;
	protected SimpleTurnPath postActionMovePath;
	protected float preActionTilt = 0;
	protected float postActionTilt = 0;

	protected List<IPath> previousMovePaths = new List<IPath>();
	protected List<float> previousTilts = new List<float>();

	// body animations
	protected Transform model = null;
	protected Transform projectedModel = null;
	protected QuickerEffects.Outline selectionOutline = null;
	protected QuickerEffects.Overlay damageOverlay = null;

	// particle effects
	protected ParticleSystem hitEffect = null;
	protected ParticleSystem attackEffect = null;
	protected ParticleSystem smokeEffect = null;
	protected ParticleSystem explosionEffect = null;

	// selection
	protected Collider selectionModelCollider = null;
	protected Collider selectionUICollider = null;

	// procedual display
	protected HeightMeshDisplay currentHeightDisplay;
	protected MovementMeshDisplay currentMoveFullDisplay;
	protected MovementMeshDisplay currentMoveAreaDisplay;
	protected HeightMeshDisplay nextHeightDisplay;
	protected MovementMeshDisplay nextMoveFullDisplay;
	protected MovementMeshDisplay nextMoveAreaDisplay;
	protected PathMeshDisplay movePathDisplay;
	protected AttackMeshDisplay attackAreaDisplay;
	protected TrailMeshDisplay trailsDisplay;

	// ========================================================= Monobehaviour Methods =========================================================

	/// <summary>
	/// Awake is called when the game object was created. It is always called before start and is 
	/// independent of if the game object is active or not.
	/// </summary>
	protected void Awake()
	{
		RetrieveReferences();
		RetrieveUIReferences();
	}

	/// <summary>
	/// Start is called before the first frame update and/or the game object is first active.
	/// </summary>
	protected void Start()
	{
		RegisterToComponents();
		DefineStateMachine();
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	protected void Update()
	{
		RunStateMachine();
		UpdateUIs();
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

		// retrieve animation components
		model = transform.Find("Models");
		projectedModel = transform.Find("ProjectedModels");
		selectionOutline = transform.Find("Models").GetComponent<QuickerEffects.Outline>();
		damageOverlay = transform.Find("Models").GetComponent<QuickerEffects.Overlay>();

		// retrieve selection components
		selectionModelCollider = transform.Find("Models/SelectionModelCollider").GetComponent<Collider>();
		selectionUICollider = transform.Find("3DUIs/SelectionUICollider").GetComponent<Collider>();

		// retrieve particle effect components
		hitEffect = transform.Find("Models/Effects/HitEffect").GetComponent<ParticleSystem>();
		attackEffect = transform.Find("Models/Effects/AttackEffect").GetComponent<ParticleSystem>();
		smokeEffect = transform.Find("Models/Effects/SmokeEffect").GetComponent<ParticleSystem>();
		explosionEffect = transform.Find("Models/Effects/ExplodeEffect").GetComponent<ParticleSystem>();


		// retrieve procedual display components
		currentHeightDisplay = transform.Find("MeshUIs/CurrentMoveHeightDisplay").GetComponent<HeightMeshDisplay>();
		currentMoveFullDisplay = transform.Find("MeshUIs/CurrentMoveFullDisplay").GetComponent<MovementMeshDisplay>();
		currentMoveAreaDisplay = transform.Find("MeshUIs/CurrentMoveAreaDisplay").GetComponent<MovementMeshDisplay>();
		nextHeightDisplay = transform.Find("MeshUIs/NextMoveHeightDisplay").GetComponent<HeightMeshDisplay>();
		nextMoveFullDisplay = transform.Find("MeshUIs/NextMoveFullDisplay").GetComponent<MovementMeshDisplay>();
		nextMoveAreaDisplay = transform.Find("MeshUIs/NextMoveAreaDisplay").GetComponent<MovementMeshDisplay>();
		movePathDisplay = transform.Find("MeshUIs/MovePathDisplay").GetComponent<PathMeshDisplay>();
		attackAreaDisplay = transform.Find("MeshUIs/AttackAreaDisplay").GetComponent<AttackMeshDisplay>();
		trailsDisplay = transform.Find("MeshUIs/TrailMeshDisplay").GetComponent<TrailMeshDisplay>();

	}

	/// <summary>
	/// Register this unit to higher level components.
	/// </summary>
	protected void RegisterToComponents()
	{
		playerInput.RegisterUnit(this, selectionModelCollider);
		playerInput.RegisterUnit(this, selectionUICollider);
		level.RegisterUnit(this, uiCanvas);
	}

	/// <summary>
	/// Deregister this unit from higher level components.
	/// </summary>
	protected void DeregisterFromComponents()
	{
		playerInput.DeregisterUnit(this, selectionModelCollider);
		playerInput.DeregisterUnit(this, selectionUICollider);
		level.DeregisterUnit(this, uiCanvas);
	}	
}