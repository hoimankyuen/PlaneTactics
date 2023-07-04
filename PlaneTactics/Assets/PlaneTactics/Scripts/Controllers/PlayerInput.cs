using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct PolarPlaneCastHit
{
	public float distance;
	public float azimuth;
}

public class PlayerInput : MonoBehaviour
{
	// references
	protected LevelController level = null;

	// working variables
	protected Dictionary<Collider, PlayerUnit> registeredUnit = new Dictionary<Collider, PlayerUnit>();

	protected bool[] draggingAttempt = new bool[] { false, false, false };
	protected bool[] dragging = new bool[] { false, false, false };
	protected Vector3[] pastDraggingPoint = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero };
	protected Vector3[] draggingDelta = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero};

	protected Ray mouseRay = new Ray();
	protected bool inspectingHit = false;
	protected RaycastHit insepectingHitInfo = new RaycastHit();

	// ========================================================= Monobehaviour Methods =========================================================

	protected void Awake()
	{
		level = FindObjectOfType<LevelController>();
	}
	
	// Start is called before the first frame update
	void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		DetectDragging();
		DetectPlayerSelection();
	}

	// ========================================================= Registery =========================================================

	public void RegisterUnit(PlayerUnit unit, Collider collider)
	{
		if (!registeredUnit.ContainsKey(collider))
		{
			registeredUnit[collider] = unit;
		}
	}

	public void DeregisterUnit(PlayerUnit unit, Collider collider)
	{
		if (registeredUnit.ContainsKey(collider))
		{
			registeredUnit.Remove(collider);
		}
	}

	// ========================================================= Input Detection =========================================================

	protected void DetectDragging()
	{
		for (int i = 0; i < 3; i++)
		{
			if (!draggingAttempt[i] && !EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(i))
			{			
				draggingAttempt[i] = true;
				dragging[i] = false;
				pastDraggingPoint[i] = Input.mousePosition;
				draggingDelta[i] = Vector3.zero;
			}
			if (draggingAttempt[i] && Input.GetMouseButton(i))
			{
				draggingDelta[i] = Input.mousePosition - pastDraggingPoint[i];
				pastDraggingPoint[i] = Input.mousePosition;
				if (draggingDelta[i].magnitude > 1f)
				{
					dragging[i] = true;
				}
			}
			if (draggingAttempt[i] && Input.GetMouseButtonUp(i))
			{
				dragging[i] = false;
				draggingAttempt[i] = false;
			}
		}
	}

	protected void DetectPlayerSelection()
	{
		mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		inspectingHit = Physics.Raycast(mouseRay, out insepectingHitInfo, 100000, LayerMask.GetMask("UnitSelection"), QueryTriggerInteraction.Collide);
	}

	// ========================================================= General Inqurery =========================================================

	public bool IsPressedOnScene()
	{
		if (!EventSystem.current.IsPointerOverGameObject() && !dragging[0] && !dragging[1] && !dragging[2])
		{
			return Input.GetMouseButtonUp(0);
		}
		return false;
	}

	public Vector3 GetDraggingOnSceneWorldDelta(int mouseIndex)
	{
		if (dragging[mouseIndex])
		{
			return draggingDelta[mouseIndex];
		}
		else
		{
			return Vector3.zero;
		}
	}

	// ========================================================= Unit Inqurery =========================================================

	public PlayerUnit AnySelected()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (inspectingHit)
			{
				if (Input.GetMouseButtonUp(0))
				{
					if (registeredUnit.ContainsKey(insepectingHitInfo.collider))
					{
						return registeredUnit[insepectingHitInfo.collider];
					}
				}
			}
		}
		return null;
	}

	public PlayerUnit AnyHovering()
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (inspectingHit)
			{
				if (registeredUnit.ContainsKey(insepectingHitInfo.collider))
				{
					return registeredUnit[insepectingHitInfo.collider];
				}
			}
		}
		return null;
	}

	public bool IsSelected(PlayerUnit unit)
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			return unit.Equals(AnySelected());
		}
		return false;
	}

	public bool IsHovering(PlayerUnit unit)
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			return unit.Equals(AnyHovering());
		}
		return false;
	}

	// ========================================================= Casting =========================================================
	
	public bool PolarPlaneCast(Transform transform, float height, out PolarPlaneCastHit hit)
	{
		if (!EventSystem.current.IsPointerOverGameObject())
		{
			if (new Plane(transform.up, transform.position + transform.up * height).Raycast(mouseRay, out float enter))
			{
				// returns a local polar coorinate of the mouse point on the local xz plane, with (0, 0) as forward, and azimuth in degrees
				Vector3 mouseLocalPosition = transform.InverseTransformPoint(mouseRay.GetPoint(enter)) - Vector3.up * height;
				hit.distance = Vector3.Distance(mouseLocalPosition, Vector3.zero);
				hit.azimuth = Vector3.SignedAngle(Vector3.forward, mouseLocalPosition, Vector3.up);
				return true;
			}
		}
		hit.distance = 0;
		hit.azimuth = 0;
		return false;
	}
}
