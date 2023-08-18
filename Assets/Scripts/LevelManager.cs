using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> allStones; // List containing all stone game objects
    public float heightGoal; // The height the player needs to achieve
    public ParticleSystem particleEffect; // Particle effect to play when the goal is reached

    private bool isStackLocked = false; // Flag to determine if the stack has been locked

    // This method checks the height of the topmost stone against the height goal
    public void CheckStackHeight()
    {
        // Assuming the last stone in the list is the topmost one
        float topStoneHeight = allStones[allStones.Count - 1].transform.position.y;

        if (topStoneHeight >= heightGoal && !isStackLocked)
        {
            LockStack();
        }
    }

    // This method locks the physics of all stones and plays the particle effect
    public void LockStack()
    {
        foreach (GameObject stone in allStones)
        {
            Rigidbody rb = stone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Freezes the physics of the stone
            }
        }

        // Play the particle effect
        particleEffect.Play();

        isStackLocked = true; // Flag the stack as locked
    }
}
