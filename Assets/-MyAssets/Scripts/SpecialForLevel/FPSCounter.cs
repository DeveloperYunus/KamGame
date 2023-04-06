using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI fpsTxt;
    float timer;

    public Texture2D cursorTexture;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0.2f;
            fpsTxt.text = (1 / Time.unscaledDeltaTime).ToString();
        }
    }
}
