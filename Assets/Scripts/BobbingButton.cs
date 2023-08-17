using UnityEngine;

public class BobbingButton : MonoBehaviour
{
    public float frequency = 1f;
    public float velocity = 1f;
    public float randomness = 0.5f;

    private float offset;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        offset = Random.Range(0f, 2f * Mathf.PI);
    }

    void Update()
    {
        float yVariation = Mathf.Sin(Time.time * frequency + offset) * velocity;
        float randomValue = (Random.value * 2f - 1f) * randomness;
        transform.position = initialPosition + new Vector3(0f, yVariation + randomValue, 0f);
    }
}
