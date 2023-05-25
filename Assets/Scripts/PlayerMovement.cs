using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] Transform weapon;
    [SerializeField] FlailHead flailHead;

    [SerializeField] float rotationSpeed = 75;
    [SerializeField] float speed = 3;

    [SerializeField] Rigidbody rb;

    void Start()
    {
    }

    void Update()
    {

        // Movement
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Move the player in the direction of the input.
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        movement *= speed * Time.deltaTime;
        transform.Translate(movement);

        // Get the mouse position in world space.
        Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        weapon.localPosition = new Vector3(((mousePosition.x - 0.5f) * 2), (mousePosition.y + 1), 1);

        if(Input.GetMouseButton(0))
        {
            weapon.localRotation = GetWeaponDirection(mousePosition);
            flailHead.swing = true;
        }
        else
        {
            weapon.localRotation = Quaternion.Slerp(weapon.localRotation, Quaternion.AngleAxis(90, Vector3.right), 5 * Time.deltaTime);
            flailHead.swing = false;
        }

        // Check if the mouse is touching the edge of the screen.
        if (mousePosition.x < 0) { transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0); }
        if (mousePosition.x > 1) { transform.Rotate(0, rotationSpeed * Time.deltaTime, 0); }

    }

    Quaternion GetWeaponDirection(Vector3 mousePosition)
    {
        // Calculate the desired rotation.
        Quaternion desiredRotation = Quaternion.identity;

        //top
        if (mousePosition.x < 0.7f && mousePosition.x > 0.3f && mousePosition.y > 0.7f)
        {
            desiredRotation = Quaternion.AngleAxis(0, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        //top left
        else if (mousePosition.x < 0.3f && mousePosition.y > 0.7f)
        {
            desiredRotation = Quaternion.AngleAxis(45, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        //left 
        else if (mousePosition.x < .3 && mousePosition.y < 0.7f && mousePosition.y > 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(90, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        //bottom left
        else if (mousePosition.x < 0.3f && mousePosition.y < 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(125, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        //bottom right
        else if (mousePosition.x > 0.7f && mousePosition.y < 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(225, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        //right
        else if (mousePosition.x > 0.7f && mousePosition.y < 0.7f && mousePosition.y > 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(270, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        //top right
        else if (mousePosition.x > 0.7f && mousePosition.y > 0.7f)
        {
            desiredRotation = Quaternion.AngleAxis(315, Vector3.forward);
            desiredRotation *= Quaternion.AngleAxis(-60, Vector3.right);
        }
        else
        { 
            desiredRotation = Quaternion.AngleAxis(90, Vector3.right);
            return Quaternion.Slerp(weapon.localRotation, desiredRotation, 0.02f);
        }
        
        return Quaternion.Slerp(weapon.localRotation, desiredRotation, Time.deltaTime * 8);
    }

}
