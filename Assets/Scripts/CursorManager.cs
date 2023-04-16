using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    static Texture2D normalCursor;
    static Texture2D noRangeCursor;
    static string normalCursorPath = "Cursors/NormalArrow";
    static string noRangeCursorPath = "Cursors/NoRangeArrow";
    public Transform playerBodyPosition;
    public const int playerTileRange = 5; // Max destroy/set tile range
    public static bool playerHasRange = false;

    // Start is called before the first frame update
    void Start()
    {
        SetNormalCursor();
    }

    void Update()
    {
        //// Check if plater have range also change cursor texture according to that range ////

        // Get mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        // Count player-mouse distance in int
        int x = Mathf.FloorToInt(Mathf.Abs(mousePos2D.x - playerBodyPosition.position.x));
        int y = Mathf.FloorToInt(Mathf.Abs(mousePos2D.y - playerBodyPosition.position.y));
        int r = Mathf.FloorToInt(Mathf.Sqrt(x * x + y * y));

        // Player doesn't have range
        if (r > playerTileRange)
        {
            CursorManager.SetNoRangeCursor();
            playerHasRange = false;
        }
        // Player has range
        else if (r <= playerTileRange)
        {
            CursorManager.SetNormalCursor();
            playerHasRange = true;
        }
    }

    public static void SetNoRangeCursor()
    {
        noRangeCursor = Resources.Load<Texture2D>(noRangeCursorPath);
        if (noRangeCursor != null) Cursor.SetCursor(noRangeCursor, Vector2.zero, CursorMode.Auto);
    }
    public static void SetNormalCursor()
    {
        normalCursor = Resources.Load<Texture2D>(normalCursorPath); 
        if (normalCursor != null) Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }

    public static void SetWithItemCursor()
    {
        normalCursorPath = "Cursors/NormalWithItemArrow";
        noRangeCursorPath = "Cursors/NoRangeWithItemArrow";
    }

    public static void SetNoItemCursor()
    {
        normalCursorPath = "Cursors/NormalArrow";
        noRangeCursorPath = "Cursors/NoRangeArrow";
    }
}
