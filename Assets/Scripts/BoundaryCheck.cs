using UnityEngine;

public class BoundaryCheck : MonoBehaviour
{
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position; // Save the initial position
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rock"))
        {
            // Reset rock's position to its original position
            other.transform.position = originalPosition;

            // Additionally, if you want to reset rock's velocity
            Rigidbody rockRb = other.GetComponent<Rigidbody>();
            if (rockRb)
            {
                rockRb.velocity = Vector3.zero;
                rockRb.angularVelocity = Vector3.zero;
            }
        }
    }
}