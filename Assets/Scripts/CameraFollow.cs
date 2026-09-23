using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Drag Mario here in the Inspector
    public float fixedY = 0f; // Set this to the static height you want
    public float fixedZ = -10f; // Standard 2D camera depth

    void LateUpdate()
    {
        if (target != null)
        {
            // Follow Mario's X position, but lock Y and Z
            transform.position = new Vector3(target.position.x + 5, fixedY, fixedZ);
        }
    }
}