using UnityEngine;
using TMPro;

public class BobbingTMP : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float frequency = 1f;  // How fast the TMP bobs up and down
    public float velocity = 1f;   // How high or low the TMP goes in each bob

    [Header("Randomness")]
    public float randomness = 0.5f;  // How much randomness to add (0 for no randomness, 1 for max randomness)

    private float offset;           // Offset to ensure different TMP elements are out of sync
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
        offset = Random.Range(0f, 2f * Mathf.PI);  // Initialize with a random phase
    }

    private void Update()
    {
        float yVariation = Mathf.Sin(Time.time * frequency + offset) * velocity;
        float randomValue = (Random.value * 2f - 1f) * randomness;  // Generates a random value between -1 and 1 multiplied by randomness
        transform.position = initialPosition + new Vector3(0f, yVariation + randomValue, 0f);
    }
}
