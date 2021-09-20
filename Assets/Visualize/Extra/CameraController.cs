using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform pivot;
	public float rotSpeed = 6;
	public float zoomSpeed = 6;

	float x;
	float y;

	void Start () {
		x = transform.eulerAngles.x;
		y = transform.eulerAngles.y;
	}

	void LateUpdate () {
		if (Input.GetKey (KeyCode.LeftAlt) && Input.GetMouseButton (0)) {
			Vector2 mouseInput = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));

			if (Input.GetKey (KeyCode.LeftControl)) {
				float zoomDir = Mathf.Sign (mouseInput.x);
				float zoomAmount = mouseInput.magnitude * zoomSpeed * zoomDir;
				transform.position += transform.forward * zoomAmount;

			} else {
				x += mouseInput.x * rotSpeed;
				y -= mouseInput.y * rotSpeed;

				Quaternion rotation = Quaternion.Euler (y, x, 0);
				//rotation = transform.rotation * Quaternion.Euler (-mouseInput.y * rotSpeed, mouseInput.x * rotSpeed, 0);

				Vector3 position = rotation * Vector3.forward * -(pivot.position - transform.position).magnitude + pivot.position;

				transform.rotation = rotation;
				transform.position = position;
			}
		}
	}
}