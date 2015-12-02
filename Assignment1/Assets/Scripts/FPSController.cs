﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FPSController : MonoBehaviour {

	Vector3 position;
	Vector3 look;
    Vector3 right;
    Vector3 up;
    Quaternion orientation;
    public float speed = 100.0f;
    float mouseX, mouseY;


    // Use this for initialization
	void Start () 
	{ 
	}

	void Yaw(float angle)
	{
		Quaternion rot = Quaternion.AngleAxis (angle, Vector3.up);
		orientation = rot * orientation;
	}

	void Pitch(float angle)
	{
		float invcosTheta1 = Vector3.Dot(look, Vector3.up);
		float threshold = 0.95f;
		if ((angle > 0 && invcosTheta1 < (-threshold)) || (angle < 0 && invcosTheta1 > (threshold)))
		{
			return;
		}

		// A pitch is a rotation around the right vector
		Quaternion rot = Quaternion.AngleAxis(angle, right);

		orientation = rot * orientation;
	}

	void UpdateMe()
	{
		position = gameObject.transform.position;
		orientation = gameObject.transform.rotation;
		look = gameObject.transform.forward;
		right = gameObject.transform.right;
        up = gameObject.transform.up;
	}

	void UpdateGameObject()
	{
		gameObject.transform.position = position;
		gameObject.transform.rotation = orientation;
	}
	// Update is called once per frame
	void Update () 
	{
        float speed = this.speed;

		UpdateMe();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= 3.0f;
        }
        
        if (Input.GetKey(KeyCode.W)) 
		{
			position +=  gameObject.transform.forward * Time.deltaTime* speed;
		}

        if (Input.GetKey(KeyCode.S))
        {
            position -= gameObject.transform.forward * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            position -= gameObject.transform.right * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            position += gameObject.transform.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.R))
        {
            position += gameObject.transform.up * Time.deltaTime * speed;
        }

        if (Input.GetKey(KeyCode.F))
        {
            position -= gameObject.transform.up * Time.deltaTime * speed;
        }




        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

		float invcosTheta1 = Vector3.Dot(look, Vector3.up);
		float angle = Mathf.Acos (invcosTheta1);

		Yaw(mouseX);
		Pitch(-mouseY);

		UpdateGameObject();
	}
}