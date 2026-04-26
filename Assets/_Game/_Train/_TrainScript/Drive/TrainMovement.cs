using UnityEngine;
using System.Collections.Generic;

public class TrainMovement : MonoBehaviour
{
    public TrainZoneDrive TrainZoneDrive; // Referenca na zelenu kocku
    public MonoBehaviour PlayerMovementScriptRef; // <--- NOVO JAVNO POLJE ZA TVOJU SKRIPTU IGRACA

    public List<Transform> waypoints;
    public float maxSpeed = 10f;
    public float acceleration = 2f;
    public float deceleration = 1f;
    public float stopThreshold = 0.5f;

    private float currentSpeed = 0f;
    private int currentWaypointIndex = 0;

    private bool vozimoVoz = false; // NOVO: prati da li je E pritisnuto

    void Update()
    {
        // Logika za preuzimanje/napuštanje kontrole (E taster)
        if (TrainZoneDrive != null && TrainZoneDrive.IgracJeUnutra)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                vozimoVoz = !vozimoVoz; // Menja stanje vožnje

                if (vozimoVoz)
                {
                    // FUNKCIJE: Iskljuci kretanje igraca, zakaci ga za voz
                    if (PlayerMovementScriptRef != null)
                    {
                        PlayerMovementScriptRef.enabled = false; // Iskljuci skriptu igraca
                    }
                    // Zakaci igraca za voz (parenting)
                    GameObject.FindWithTag("Player").transform.SetParent(this.transform);
                }
                else
                {
                    // FUNKCIJE: Ukljuci kretanje igraca, odlepi ga
                    if (PlayerMovementScriptRef != null)
                    {
                        PlayerMovementScriptRef.enabled = true; // Ukljuci skriptu igraca
                    }
                    // Odlepi igraca od voza
                    GameObject.FindWithTag("Player").transform.SetParent(null);
                }
            }
        }
        else if (!TrainZoneDrive.IgracJeUnutra && vozimoVoz)
        {
            // Ako izadjemo iz zone dok vozimo, automatski resetuj stanje
            vozimoVoz = false;
            if (PlayerMovementScriptRef != null)
            {
                PlayerMovementScriptRef.enabled = true;
            }
            GameObject.FindWithTag("Player").transform.SetParent(null);
        }

        // Upravljanje brzinom: Kretanje je moguće samo ako je 'vozimoVoz' true
        if (vozimoVoz && Input.GetKey(KeyCode.W))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (vozimoVoz && Input.GetKey(KeyCode.S))
        {
            currentSpeed -= acceleration * Time.deltaTime;
        }
        // ... ostatak logike za usporavanje ...
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= deceleration * Time.deltaTime;
                currentSpeed = Mathf.Max(currentSpeed, 0f);
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += deceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, 0f);
            }
        }

        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // ... Ostatak koda za kretanje duž putanje je isti ...
        if (waypoints.Count == 0) return;

        int moveDir = (currentSpeed >= 0) ? 1 : -1;

        // target za KRETANJE
        int moveTargetIndex = currentWaypointIndex;
        if (moveDir == -1)
        {
            moveTargetIndex = currentWaypointIndex - 1;
            if (moveTargetIndex < 0) moveTargetIndex = waypoints.Count - 1;
        }

        // target za ROTACIJU (uvek napred)
        int rotationTargetIndex = currentWaypointIndex;
        if (rotationTargetIndex >= waypoints.Count) rotationTargetIndex = 0;

        Transform moveTarget = waypoints[moveTargetIndex];
        Transform rotationTarget = waypoints[rotationTargetIndex];

        // KRETANJE
        transform.position = Vector3.MoveTowards(
            transform.position,
            moveTarget.position,
            Mathf.Abs(currentSpeed) * Time.deltaTime
        );

        // ROTACIJA (uvek napred, nema 180 flip)
        Vector3 rotDir = rotationTarget.position - transform.position;
        if (rotDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(rotDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * (Mathf.Abs(currentSpeed) / 2f)
            );
        }

        // Kad stigneš do targeta kretanja, menjaj index
        if (Vector3.Distance(transform.position, moveTarget.position) < stopThreshold)
        {
            currentWaypointIndex += moveDir;

            if (currentWaypointIndex >= waypoints.Count) currentWaypointIndex = 0;
            else if (currentWaypointIndex < 0) currentWaypointIndex = waypoints.Count - 1;
        }
    }
}
