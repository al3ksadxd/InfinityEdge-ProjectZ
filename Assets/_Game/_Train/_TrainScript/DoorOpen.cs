using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator; // Assign in Inspector
    public float openDelay = 1f; // Time in seconds before door opens

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start a coroutine to open the door after delay
            StartCoroutine(OpenDoorAfterDelay());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Immediately close the door when player leaves
            doorAnimator.SetBool("OpenDoor", false);
            // Stop any pending open if player leaves early
            StopAllCoroutines();
        }
    }

    private IEnumerator OpenDoorAfterDelay()
    {
        yield return new WaitForSeconds(openDelay); // Wait for the delay
        doorAnimator.SetBool("OpenDoor", true); // Open the door
    }
}
