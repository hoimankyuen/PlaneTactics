using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public partial class LevelController : MonoBehaviour
{
	// level parameters
	[Header("Level Parameters")]
	public RangeFloat2 altitudeRange = new RangeFloat2(1, 5);
	public float altitudeHeight = 10f;

	// references
	protected CameraController cameraControl = null;
	protected List<TeamInfo> teams = new List<TeamInfo>();
	protected Dictionary<TeamInfo, List<PlayerUnit>> units = new Dictionary<TeamInfo, List<PlayerUnit>>();
	protected List<Building> buildings = new List<Building>();

	// working variables
	public int CurrentTurn { get; protected set; } = -1;
	public int ActiveTeamIndex { get; protected set; } = -1;
	public TeamInfo ActiveTeam { get { return ActiveTeamIndex == -1 ? null : teams[ActiveTeamIndex]; } }
	public PlayerUnit ActiveUnit { get; set; } = null;
	public PlayerUnit TargetUnit { get; set; } = null;

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
	void Start()
	{
		DefineStateMachine();
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	void Update()
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
		cameraControl = FindObjectOfType<CameraController>();		
	}

	// ========================================================= Registration =========================================================

	/// <summary>
	/// Regeister a unit and its canvas to this level.
	/// </summary>
	public void RegisterUnit(PlayerUnit unit, Canvas canvas)
	{
		// add team if needed
		if (!units.ContainsKey(unit.teamInfo))
		{
			units[unit.teamInfo] = new List<PlayerUnit>();
			teams.Add(unit.teamInfo);
			teams.Sort((x, y) => { return x.priority - y.priority; });
		}

		// add unit if possible
		if (!units[unit.teamInfo].Contains(unit))
		{
			units[unit.teamInfo].Add(unit);

			canvas.GetComponent<RectTransform>().SetParent(uiCanvas.transform.Find("WorldElements"), false);
			canvas.transform.localPosition = Vector3.zero;
			canvas.transform.localRotation = Quaternion.identity;

			RectTransform newUnitMarkerTransform = Instantiate(minimapUnitMarkerPrefab).GetComponent<RectTransform>();
			newUnitMarkerTransform.transform.SetParent(minimapUnitMarkerPrefab.transform.parent, false);
			newUnitMarkerTransform.transform.SetSiblingIndex(newUnitMarkerTransform.parent.childCount - 2);
			newUnitMarkerTransform.GetComponent<Image>().color = unit.teamInfo.color;
			newUnitMarkerTransform.gameObject.SetActive(true);
			minimapUnitMarkers.Add(new Tuple<PlayerUnit, RectTransform>(unit, newUnitMarkerTransform));
		}
	}

	/// <summary>
	/// Deregeister a unit and its canvas from this level.
	/// </summary>
	public void DeregisterUnit(PlayerUnit unit, Canvas canvas)
	{
		// remove unit
		if (units[unit.teamInfo].Contains(unit))
		{
			units[unit.teamInfo].Remove(unit);

			Destroy(canvas);

			Tuple<PlayerUnit, RectTransform> mapUnitMarker = minimapUnitMarkers.Find(x => x.Item1 == unit);
			Destroy(mapUnitMarker.Item2);
			minimapUnitMarkers.Remove(mapUnitMarker);
		}

		// remove team if possible
		if (units.ContainsKey(unit.teamInfo) && units[unit.teamInfo].Count == 0)
		{
			teams.Remove(unit.teamInfo);
		}
	}

	/// <summary>
	/// Register a building.
	/// </summary>
	public void RegisterBuilding(Building building)
	{
		// add building if possible
		if (!buildings.Contains(building))
		{
			buildings.Add(building);
		}
	}

	/// <summary>
	/// Deregister a building.
	/// </summary>
	public void DeregisterBuilding(Building building)
	{
		// add building if possible
		if (buildings.Contains(building))
		{
			buildings.Remove(building);
		}
	}

	// =========================================================  Inqurery =========================================================

	/// <summary>
	/// Get all units with a certain radius of a particular point.
	/// </summary>
	public List<PlayerUnit> GetAllUnitsInPoint(Vector3 point, float radius)
	{
		List<PlayerUnit> returnList = new List<PlayerUnit>();
		foreach (KeyValuePair<TeamInfo, List<PlayerUnit>> allUnitKVP in units)
		{
			returnList.AddRange(allUnitKVP.Value.Where(x => Vector3.Distance(x.gameObject.transform.position, point) < (x.unitInfo.hitboxRadius + radius)));
		}
		return returnList;
	}
}



