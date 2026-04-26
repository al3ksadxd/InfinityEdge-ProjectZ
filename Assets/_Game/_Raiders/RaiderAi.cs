using UnityEngine;
using UnityEngine.AI;

public class RaiderAi : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform igrac;
    private bool isDead = false; // Da ne moze da umre dva puta

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) igrac = playerObj.transform;
    }

    void Update()
    {
        // Ako je mrtav, ne radi nista
        if (isDead || igrac == null) return;

        agent.SetDestination(igrac.position);
    }

    // Ovu funkciju pozivamo iz WeaponManager-a
    public void TakeDamage()
    {
        if (isDead) return;

        isDead = true;
        agent.enabled = false; // Prestaje da juri

        // Simulacija ragdoll-a za kocku:
        // Dodajemo Rigidbody u trenutku smrti da bi pala
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();

        // Bacamo je malo unazad od siline metka
        rb.AddForce(-transform.forward * 5f, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse); // Malo je zarotiramo da padne "trapavo"

        Debug.Log("Raider je oboren!");
    }
}