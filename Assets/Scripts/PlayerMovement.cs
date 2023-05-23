using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    [SerializeField] Transform weapon;

    [SerializeField] float rotationSpeed = 75;
    [SerializeField] float speed = 3;

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

        // Move weapon with mouse.
        weapon.localPosition = new Vector3(((mousePosition.x - 0.5f) * 2), (mousePosition.y + 1), 1);
        weapon.localRotation = GetWeaponDirection(mousePosition);


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
        }
        //top left
        else if (mousePosition.x < 0.3f && mousePosition.y > 0.7f)
        {
            desiredRotation = Quaternion.AngleAxis(45, Vector3.forward);
        }
        //left 
        else if (mousePosition.x < .3 && mousePosition.y < 0.7f && mousePosition.y > 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(90, Vector3.forward);
        }
        //bottom left
        else if (mousePosition.x < 0.3f && mousePosition.y < 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(125, Vector3.forward);
        }
        //bottom right
        else if (mousePosition.x > 0.7f && mousePosition.y < 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(225, Vector3.forward);
        }
        //right
        else if (mousePosition.x > 0.7f && mousePosition.y < 0.7f && mousePosition.y > 0.3f)
        {
            desiredRotation = Quaternion.AngleAxis(270, Vector3.forward);
        }
        //top right
        else if (mousePosition.x > 0.7f && mousePosition.y > 0.7f)
        {
            desiredRotation = Quaternion.AngleAxis(315, Vector3.forward);
        }
        else
        {
            desiredRotation = Quaternion.AngleAxis(60, Vector3.right);
        }

        // Calculate the interpolation amount.
        float t = Time.deltaTime;

        return Quaternion.Slerp(weapon.localRotation, desiredRotation, t * 8);
    }

}
