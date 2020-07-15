using System;
using UnityEngine;
using UnityEngine.Tilemaps;

using Constants;

public class TilemapMonobehavior : GameObjectMonobehavior
{
    public TileBase OnTile;
    public TileBase OffTile;

    private GridData GridData;
    private Tilemap Tilemap;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap = gameObject.GetComponent<Tilemap>();
        GridData = GM.GridData;
    }

    public void ShowGridAtView(Rect view_rect) {
        RectOffset padding = new RectOffset(3, 3, 3, 3);
        view_rect = padding.Add(view_rect);

        int xMin = (int)Math.Floor(view_rect.xMin);
        int yMin = (int)Math.Floor(view_rect.yMin);
        int xMax = (int)Math.Ceiling(view_rect.xMax);
        int yMax = (int)Math.Ceiling(view_rect.yMax);


        for (int x = xMin; x <= xMax; x++) {
            for (int y = yMin; y <= yMax; y++) {
                Vector3Int tile_loc = new Vector3Int(x, y, 0);
                TM_Symbol cellSymbol = GridData.GetCellSymbol(tile_loc);
                TileBase cellTile;
                switch (cellSymbol)
                {
                    case TM_Symbol.OFF:
                        cellTile = OffTile;
                        break;
                    case TM_Symbol.ON:
                        cellTile = OnTile;
                        break;
                    default:
                        cellTile = OffTile;
                        break;
                }
                Tilemap.SetTile(tile_loc, cellTile);
            }
        }
    }

    public void ResetBoard()
    {
        Tilemap.ClearAllTiles();
    }
}
