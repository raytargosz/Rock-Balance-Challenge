using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ObjectEnabler : MonoBehaviour
{
    [Header("Objects List")]
    [Tooltip("List of GameObjects to be enabled sequentially")]
    public GameObject[] objectsToEnable;

    [Header("Initial Settings")]
    [Tooltip("Initial delay before starting the process")]
    public float initialDelay = 5f;
    [Tooltip("Time interval between enabling objects")]
    public float interval = 1f;

    [Header("Ramp Up Settings")]
    [Tooltip("Enable ramping up the speed?")]
    public bool isRampUpTrue = false;
    [Tooltip("Decrease amount for each interval if ramp up is true")]
    public float rampUpSpeed = 0.05f; // The speed at which it ramps up

    // Internal variables
    private float currentInterval;  // The current interval to be used

    void Start()
    {
        // Initialize all objects to be disabled
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(false);
        }

        currentInterval = interval;
        StartCoroutine(EnableObjects());
    }

    IEnumerator EnableObjects()
    {
        yield return new WaitForSeconds(initialDelay);

        int totalObjects = objectsToEnable.Length;
        int remainingObjects = totalObjects;

        for (int i = 0; i < totalObjects; i++)
        {
            objectsToEnable[i].SetActive(true);  // Activate the GameObject
            remainingObjects--;

            // If ramp up is true and current interval is greater than the minimum value
            if (isRampUpTrue && currentInterval > 0.25f)
            {
                // Update the interval based on the percentage of remaining objects 
                // to the total and the ramp up speed
                currentInterval -= (remainingObjects / (float)totalObjects) * rampUpSpeed;
                currentInterval = Mathf.Clamp(currentInterval, 0.25f, interval); // Ensure it's between 0.25 and initial interval
            }

            yield return new WaitForSeconds(currentInterval);
        }
    }
}
