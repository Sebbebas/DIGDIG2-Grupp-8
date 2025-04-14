using UnityEngine;

public class CursorTextureMain : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Vector2 cursorPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.SetCursor(cursorTexture, cursorPos, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.SetCursor(cursorTexture, cursorPos, CursorMode.Auto);
    }
}
