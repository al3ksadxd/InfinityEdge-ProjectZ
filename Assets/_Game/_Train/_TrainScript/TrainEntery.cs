using GLTFast.Schema;
using System.Collections;
using UnityEngine;

public class TrainEntry : MonoBehaviour
{
    private Collider triggerCollider;





    public float rotateSpeed = 5f;

    public Transform climbPoint;
    public Transform knobPoint;
    public Transform insideTrainPoint;

    private Transform player;
    private bool canInteract;


    public Animator animatorTrain;
    // Cached components (auto-detected)
    private CharacterController characterController;
    private Rigidbody rb;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;

            characterController = other.GetComponent<CharacterController>();
            rb = other.GetComponent<Rigidbody>();

            canInteract = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EnterTrain());
            canInteract = false;
        }
    }
    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }
    IEnumerator EnterTrain()
    {
        // Snap player to center of THIS trigger
        Vector3 center = triggerCollider.bounds.center;

        // keep player height
        player.position = new Vector3(center.x, player.position.y, center.z);


        if (characterController != null) characterController.enabled = false;
        player.position = triggerCollider.bounds.center;



        CameraController cam = player.GetComponentInChildren<CameraController>();
        Movement movementScript = player.GetComponent<Movement>();


        // 1. DISABLE CameraController script
        if (cam != null)
        {
            cam.SetCameraRotation(75f);
            cam.enabled = false;
        }

        //  LOCK CONTROLS
        if (cam != null)
        {
            cam.canLook = false;
        }


        if (movementScript != null)
        {
            movementScript.canMove = false; // immediately freeze movement
        }

        // Optional: ensure Rigidbody is frozen
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = false;// ensure no physics moves it seti it true if u need that
        }

        // 🔥 ROTATE TOWARDS TRAIN FIRST
        yield return RotatePlayerTowards(transform);




        // Move player manually

        animatorTrain.SetBool("IsClimbingTheTrain", true);
        yield return MovePlayer(climbPoint.position, 3.5f);
        yield return MovePlayer(knobPoint.position, 2f);
        yield return MovePlayer(insideTrainPoint.position, 2f);
        animatorTrain.SetBool("IsClimbingTheTrain", false);
        // Restore physics

        // 1. Enable CameraController script
        if (cam != null)
        {
            cam.enabled = true;
        }

        if (movementScript != null)
        {
            movementScript.canMove = true;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (cam != null)
        {
            cam.canLook = true;
        }
    }

    IEnumerator MovePlayer(Vector3 target, float duration)
    {
        Vector3 start = player.position;

        float time = 0f;

        while (time < duration)
        {
            player.position = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        player.position = target;
    }

    IEnumerator RotatePlayerTowards(Transform target)
    {
        while (true)
        {
            Vector3 direction = target.position - player.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f)
                break;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -10f, 0);

            player.rotation = Quaternion.Slerp(
                player.rotation,
                targetRotation,
                Time.deltaTime * rotateSpeed
            );

            if (Quaternion.Angle(player.rotation, targetRotation) < 0.01f)
                break;

            yield return null;
        }
    }
}
