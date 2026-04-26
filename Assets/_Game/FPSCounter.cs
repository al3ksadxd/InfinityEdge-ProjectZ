using UnityEngine;
using TMPro;

public partial class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    void Update()
    {
        // Računa frejmove
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // Ispisuje u uglu (npr: 60 FPS)
        fpsText.text = string.Format("{0:0.} FPS", fps);

        // Boja: Ako je ispod 30 FPS neka bude crveno, ako je preko 60 zeleno
        if (fps < 30) fpsText.color = Color.red;
        else if (fps < 60) fpsText.color = Color.yellow;
        else fpsText.color = Color.green;
    }
}