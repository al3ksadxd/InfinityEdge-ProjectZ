using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [Header("Podešavanja Kamera")]
    public Camera mainCamera;      // Tvoja glavna kamera (iz prvog lica)
    public Camera inventoryCamera; // Ona nova kamera koju si napravio za inventar

    [Header("Tasteri")]
    public KeyCode inventoryKey = KeyCode.I; // Možeš promeniti u Tab ili bilo šta drugo

    private bool isInventoryOpen = false;

    void Start()
    {
        // Na samom početku igre, ugasimo inventar kameru
        inventoryCamera.enabled = false;

        // Osigurajmo da je miš zaključan na početku (za tvoj shooter)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Proveravamo da li je igrač pritisnuo taster za inventar
        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        // Obrnemo stanje (ako je bio zatvoren, sad se otvara i obrnuto)
        isInventoryOpen = !isInventoryOpen;

        // 1. Palimo/gasimo kameru inventara
        inventoryCamera.enabled = isInventoryOpen;

        // 2. Kontrola miša
        if (isInventoryOpen)
        {
            Cursor.lockState = CursorLockMode.None; // Otključaj miš da možeš da klikćeš
            Cursor.visible = true;                  // Prikaži kursor
            Time.timeScale = 0f;                    // Pauziraj igru (kao u The Forest)
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Ponovo zaključaj miš za pucanje/gledanje
            Cursor.visible = false;
            Time.timeScale = 1f;                      // Vrati vreme u normalu
        }
    }
}