
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player; // Mario's Transform
    public Transform endLimit; // GameObject that indicates end of map
    private float offset; // initial x-offset between camera and Mario
    private float startX; // smallest x-coordinate of the Camera
    private float endX; // largest x-coordinate of the camera
    private float viewportHalfWidth;

    void Start()  //Set camera boundaries from start
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));

        viewportHalfWidth = Mathf.Abs(bottomLeft.x - this.transform.position.x);

        offset = this.transform.position.x - player.position.x;

        startX = this.transform.position.x;

        endX = endLimit.transform.position.x - viewportHalfWidth;
    }

    void Update()
    {
        // Calculate where the camera should be based on Mario's position
        float desiredX = player.position.x + offset;

        //Follow mario within the boundaries
        if (desiredX > startX && desiredX < endX)
        {
            this.transform.position = new Vector3(
                desiredX,
                this.transform.position.y,
                this.transform.position.z
            );
        }
    }

}

