using UnityEngine;
using System.Collections; // Obavezno za IEnumerator (Reload)

public class WeaponManager : MonoBehaviour
{
    [Header("Reference")]
    public Camera mainCamera;
    public GameObject pistolModel;

    [Header("Municija & Reload (NOVO)")]
    public int maxAmmo = 15;          // Standardni Glock 19 kapacitet
    public int currentAmmo;
    public float reloadTime = 2.0f;    // Koliko sekundi traje tvoja animacija
    private bool isReloading = false;

    [Header("Efekti")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public GameObject bulletHolePrefab;

    [Header("Trzaj Modela")]
    public Vector3 kickbackAmount = new Vector3(0, 0, -0.1f);
    public Vector3 kickbackRotation = new Vector3(-10f, 0, 0);
    public float returnSpeed = 10f;

    private Vector3 initialPistolPos;
    private Quaternion initialPistolRot;

    [Header("Pucanje & Preciznost")]
    public float domet = 100f;
    [Range(0f, 0.2f)]
    public float hipFireSpread = 0.05f;

    [Header("Kicma & Centriranje")]
    public Transform spineBone;
    public Vector3 spineOffset;

    private CameraController camControl;
    private Animator animator;
    public bool isPistolEquipped = false;
    public bool isAiming = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentAmmo = maxAmmo;

        if (mainCamera != null) camControl = mainCamera.GetComponent<CameraController>();

        if (pistolModel != null)
        {
            // Uzimamo poziciju iako je mozda ugasen u Hierarchy-ju
            initialPistolPos = pistolModel.transform.localPosition;
            initialPistolRot = pistolModel.transform.localRotation;

            // Za svaki slucaj, potvrdjujemo da je ugasen na samom pocetku
            isPistolEquipped = false;
            pistolModel.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TogglePistol();

        if (isPistolEquipped)
        {
            isAiming = Input.GetMouseButton(1);
            animator.SetBool("IsGlock19Aim", isAiming);
            if (camControl != null) camControl.isLockedForAim = isAiming;

            // Vracanje pistolja od trzaja
            pistolModel.transform.localPosition = Vector3.Lerp(pistolModel.transform.localPosition, initialPistolPos, Time.deltaTime * returnSpeed);
            pistolModel.transform.localRotation = Quaternion.Slerp(pistolModel.transform.localRotation, initialPistolRot, Time.deltaTime * returnSpeed);

            // --- PROVERA ZA RELOAD (Taster R) ---
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
            {
                StartCoroutine(Reload());
            }
        }
    }

    void LateUpdate()
    {
        if (isPistolEquipped && spineBone != null && camControl != null)
        {
            float targetPitch = camControl.XRotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetPitch) * Quaternion.Euler(spineOffset);
            spineBone.localRotation = targetRotation;

            // Pucamo samo ako nismo u sred reloada i imamo metaka
            if (Input.GetMouseButtonDown(0) && !isReloading)
            {
                if (currentAmmo > 0)
                {
                    Pucaj();
                }
                else
                {
                    Debug.Log("KLIK! Prazan okvir. Pritisni R!");
                    // Ovde mozes dodati zvuk "praznog klika"
                }
            }
        }
    }

    void Pucaj()
    {
        if (mainCamera == null || pistolModel == null) return;

        currentAmmo--; // Trošimo metak
        Debug.Log("Municija: " + currentAmmo + "/" + maxAmmo);

        // Trzaj
        pistolModel.transform.localPosition += kickbackAmount;
        pistolModel.transform.localRotation *= Quaternion.Euler(kickbackRotation);

        // Efekti
        if (muzzleFlash != null) muzzleFlash.Play();
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);

        // Raycast
        RaycastHit hit;
        Vector3 rayDirection = mainCamera.transform.forward;
        Vector3 spawnPoint = mainCamera.transform.position + (rayDirection * 0.5f);

        if (!isAiming)
        {
            rayDirection.x += Random.Range(-hipFireSpread, hipFireSpread);
            rayDirection.y += Random.Range(-hipFireSpread, hipFireSpread);
            rayDirection.z += Random.Range(-hipFireSpread, hipFireSpread);
            rayDirection.Normalize();
        }

        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(spawnPoint, rayDirection, out hit, domet, layerMask))
        {
            if (bulletHolePrefab != null)
            {
                GameObject hole = Instantiate(bulletHolePrefab, hit.point + (hit.normal * 0.01f), Quaternion.LookRotation(hit.normal));
                hole.transform.parent = hit.transform;
                Destroy(hole, 10f);
            }

            RaiderAi enemy = hit.transform.GetComponent<RaiderAi>();
            if (enemy != null) enemy.TakeDamage();
        }
    }

    // --- LOGIKA ZA RELOAD ---
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Punim...");

        // Pali tvoj trigger u Animatoru
        animator.SetBool("Glock19Reload", true);

        // Čekamo da se animacija završi (podesi sekunde u Inspectoru)
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        animator.SetBool("Glock19Reload", false);
        isReloading = false;
        Debug.Log("Napunjeno!");
    }

    void TogglePistol()
    {
        isPistolEquipped = !isPistolEquipped;
        if (pistolModel != null)
        {
            // Umesto SetActive, probaj ovo ako SetActive ne slusa:
            // pistolModel.GetComponent<MeshRenderer>().enabled = isPistolEquipped;

            // Standardni nacin:
            pistolModel.SetActive(isPistolEquipped);

            pistolModel.transform.localPosition = initialPistolPos;
            pistolModel.transform.localRotation = initialPistolRot;
        }
        animator.SetBool("IsGlock19Idle", isPistolEquipped);

        // Ako sklonis pistolj, prekini reload
        if (!isPistolEquipped)
        {
            isReloading = false;
            animator.SetBool("Glock19Reload", false);
            StopAllCoroutines();
        }
    }
}