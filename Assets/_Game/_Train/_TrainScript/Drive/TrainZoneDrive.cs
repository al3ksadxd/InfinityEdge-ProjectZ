using UnityEngine;

public class TrainZoneDrive : MonoBehaviour
{
    public bool IgracJeUnutra { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IgracJeUnutra = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IgracJeUnutra = false;
        }
    }

}
