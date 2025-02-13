﻿using UnityEngine;

public class CameraScript : MonoBehaviour
{
	private Transform centerFocus;
	public float degree;
	public float verticalDegree;
	public float cameraSpeed = 6;

	#region Properties
	public float Sensitivity
	{
		get { return PlayerPrefs.GetFloat("CameraSensitivity", 2f); }
	}
	#endregion

	// Use this for initialization
	public void Start()
	{
		centerFocus = GameObject.Find("CenterFocus").transform;
		verticalDegree = 30;
	}

	public void Update()
	{
		// Rotate controls
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
		{
			degree += Input.GetAxis("Mouse X") * Sensitivity * 1.2f;
		}

		degree = degree % 360;

		//Set rotation to next degree with a slight lerp
		centerFocus.rotation = Quaternion.Slerp(centerFocus.rotation, Quaternion.Euler(verticalDegree, degree, 0), Time.deltaTime * cameraSpeed);
	}
}