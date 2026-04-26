using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Paneli")]
    public GameObject pauseMenuPanel; // Glavni panel za pauzu
    public GameObject optionsPanel;   // Panel gde su tvoji slideri

    [Header("Podešavanja")]
    public TextMeshProUGUI distanceText;
    public Camera mainCam;

    private bool isPaused = false;

    void Update()
    {
        // Kad pritisneš ESC (Escape) na tastaturi
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true); // Pali meni
        Time.timeScale = 0f;            // ZAUSTAVLJA VREME (Voz staje, zombiji staju)

        // Oslobađa miš da možeš da klikćeš
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);  // Gasi i opcije ako su bile otvorene
        Time.timeScale = 1f;            // VREME PONOVO TEČE

        // Sakriva miš ponovo (za igranje)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- TVOJE FUNKCIJE OD RANIJE ---
    public void SetFPS(int index)
    {
        // Force-uj gašenje VSync-a jer on najčešće drži na 30/60
        QualitySettings.vSyncCount = 0;

        // Tvoj niz (0=30fps, 1=60fps, 2=144fps, 3=No Limit)
        int[] fpsValues = { 30, 60, 144, -1 };

        Application.targetFrameRate = fpsValues[index];

        Debug.Log("Sada je FPS limit postavljen na: " + Application.targetFrameRate);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetRenderDistance(float distance)
    {
        if (mainCam != null)
        {
            mainCam.farClipPlane = distance;
            Debug.Log("KAMERA: " + mainCam.name + " SADA VIDI: " + mainCam.farClipPlane + " metara.");
        }
        else
        {
            Debug.LogError("BRATE, NISI PREVUKAO KAMERU U SKRIPTU!");
        }
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        pauseMenuPanel.SetActive(false); // Gasi glavni pauza meni dok si u opcijama
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true); // Vraća te na glavni pauza meni
    }
}