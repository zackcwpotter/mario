using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;   // Mario's Transform
    public Transform endLimit; // Marks the right end of the level

    private float offset;            // Initial x-offset between camera and Mario
    private float startX;            // Leftmost camera position
    private float endX;              // Rightmost camera position
    private float viewportHalfWidth; // Half of the camera's visible width

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Calculate half of the camera's visible width
        viewportHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        // Keep the initial distance between Mario and the camera
        offset = transform.position.x - player.position.x;

        // Camera cannot move further left than its starting position
        startX = transform.position.x;

        // Camera stops before showing anything beyond EndLimit
        endX = endLimit.position.x - viewportHalfWidth;

        
    }

    void Update()
    {
        // Calculate where the camera should be based on Mario's position
        float desiredX = player.position.x + offset;

        // Keep the camera between the start and end of the level
        float clampedX = Mathf.Clamp(desiredX, startX, endX);

        // Follow Mario only on the x-axis
        transform.position = new Vector3(
            clampedX,
            transform.position.y,
            transform.position.z
        );
    }
}