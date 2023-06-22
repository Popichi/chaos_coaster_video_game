using UnityEngine;

public class AddForceWithClick : MonoBehaviour
{
    public float forceAmount = 50f; // The amount of force to apply

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // On left mouse button click
        {
            // Cast a ray from the camera's position towards the mouse's position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // If the ray hits an object
            {
                // Try to get a Rigidbody component on the hit object
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();

                if (rb != null) // If the hit object has a Rigidbody component
                {
                    // Apply a force to the Rigidbody
                    rb.AddForce(ray.direction * forceAmount, ForceMode.Impulse);
                }
            }
        }
    }
}