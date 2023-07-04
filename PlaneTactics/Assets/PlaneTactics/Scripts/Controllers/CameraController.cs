using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	// references
	protected PlayerInput playerInput = null;

	public Camera Main { get; protected set; }

	protected float moveSpeed = 1f;
	protected float rotateSpeed = 90f;
	protected float zoomSpeed = 1f;

	protected Coroutine gotoCoroutine = null;

	public bool LeftDragging { get; protected set; }
	public bool RightDragging { get; protected set; }

	// ========================================================= Monobehaviour Methods =========================================================

	protected void Awake()
	{
		playerInput = FindObjectOfType<PlayerInput>();

		Main = transform.Find("MainCamera").GetComponent<Camera>();
	}

	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
		KeyboardControls();
		MouseControls();

	}

	// ========================================================= Player Input =========================================================

	protected void KeyboardControls()
	{
		if (Input.GetKey(KeyCode.W))
		{
			transform.Translate(new Vector3(0, 0, 1) * moveSpeed * Vector3.Distance(Vector3.zero, Main.transform.localPosition) * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.S))
		{
			transform.Translate(new Vector3(0, 0, -1) * moveSpeed * Vector3.Distance(Vector3.zero, Main.transform.localPosition) * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.A))
		{
			transform.Translate(new Vector3(-1, 0, 0) * moveSpeed * Vector3.Distance(Vector3.zero, Main.transform.localPosition) * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.D))
		{
			transform.Translate(new Vector3(1, 0, 0) * moveSpeed * Vector3.Distance(Vector3.zero, Main.transform.localPosition) * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.Q))
		{
			transform.Rotate(new Vector3(0, 1, 0) * rotateSpeed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.E))
		{
			transform.Rotate(new Vector3(0, -1, 0) * rotateSpeed * Time.deltaTime, Space.Self);
		}
		if (Input.GetKey(KeyCode.F))
		{
			if (Vector3.Distance(Main.transform.localPosition, Vector3.zero) < 300)
			{
				Main.transform.Translate(Main.transform.forward * -1f * zoomSpeed * Vector3.Distance(Vector3.zero, Main.transform.localPosition) * Time.deltaTime, Space.World);
				Main.transform.LookAt(transform.position);
			}
		}
		if (Input.GetKey(KeyCode.R))
		{
			if (Vector3.Distance(Main.transform.localPosition, Vector3.zero) > 80)
			{
				Main.transform.Translate(Main.transform.forward * zoomSpeed * Vector3.Distance(Vector3.zero, Main.transform.localPosition) * Time.deltaTime, Space.World);
				Main.transform.LookAt(transform.position);
			}
		}
	}

	protected void MouseControls()
	{
		transform.Translate(new Vector3(0, 0, -1) * playerInput.GetDraggingOnSceneWorldDelta(2).y / 4f, Space.Self);
		transform.Translate(new Vector3(-1, 0, 0) * playerInput.GetDraggingOnSceneWorldDelta(2).x / 4f, Space.Self);
		transform.Rotate(new Vector3(0, 1, 0) * playerInput.GetDraggingOnSceneWorldDelta(1).x / 12f, Space.Self);
		if (Input.mouseScrollDelta.y > 0)
		{
			if (Vector3.Distance(Main.transform.localPosition, Vector3.zero) > 80)
			{
				Main.transform.Translate(Main.transform.forward * Input.mouseScrollDelta.y * 10f, Space.World);
				Main.transform.LookAt(transform.position);
			}
		}
		else if (Input.mouseScrollDelta.y < 0)
		{
			if (Vector3.Distance(Main.transform.localPosition, Vector3.zero) < 300)
			{
				Main.transform.Translate(Main.transform.forward * Input.mouseScrollDelta.y * 10f, Space.World);
				Main.transform.LookAt(transform.position);
			}
		}
	}

	// ========================================================= Goto =========================================================


	public void GotoPosition(Vector3 position)
	{
		if (gotoCoroutine != null)
		{
			StopCoroutine(gotoCoroutine);
		}
		gotoCoroutine = StartCoroutine(GotoPositionCoroutine(position));

		//cameraControl.transform.position = nextAvilableUnit.transform.position;
	}

	protected IEnumerator GotoPositionCoroutine(Vector3 position)
	{
		Vector3 startPosition = transform.position;
		Vector3 endPosition = position;
		float timer = 0;
		float duration = 0.25f;
		while (timer < duration)
		{
			transform.position = new Vector3(
				Mathfx.Clerp(startPosition.x, endPosition.x, timer / duration),
				Mathfx.Clerp(startPosition.y, endPosition.y, timer / duration),
				Mathfx.Clerp(startPosition.z, endPosition.z, timer / duration));
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = endPosition;
	}
}
