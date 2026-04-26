using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Osetljivost i Telo")]
    public float mouseSensitivity = 200f;
    public Transform playerBody;

    [Header("Limiti Gledanja")]
    public float minPitch = -70f;
    public float maxPitch = 75f;

    [Header("Status (Za druge skripte)")]
    public bool canLook = true; // OVO JE FALILO ZA VOZ
    [HideInInspector] public bool isLockedForAim = false;

    private float xRotation = 0f;
    public float XRotation => xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Sinhronizuj pocetnu rotaciju
        Vector3 rot = transform.localRotation.eulerAngles;
        xRotation = (rot.x > 180) ? rot.x - 360 : rot.x;
    }

    void Update()
    {
        // Ako skripta za voz kaze da ne mozes da gledas, ovde stajemo
        if (!canLook) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minPitch, maxPitch);

        // LOGIKA ZA CILJANJE (ZAKLJUCAVANJE)
        if (!isLockedForAim)
        {
            // Normalno gledanje: Kamera se vrti unutar glave
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            // ADS gledanje: Kamera gleda pravo (centrirano na nisan), 
            // a WeaponManager ce da vrti celo telo/kicmu
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 15f);
        }

        // Rotacija tela levo-desno
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void SetCameraRotation(float targetX)
    {
        xRotation = targetX;
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}