using System;
using UnityEngine;
using UnityEngine.Tilemaps;

using Constants;

public class Tilemap_Controller : MonoBehaviour
{
    public TileBase on_tile;
    public TileBase off_tile;

    private Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
        tilemap.SetTile(new Vector3Int(0, 0, 0), off_tile);
    }

    public void PopulateGridAtView(Rect view_rect) {
        RectOffset padding = new RectOffset(3, 3, 3, 3);
        view_rect = padding.Add(view_rect);

        int xMin = (int)Math.Floor(view_rect.xMin);
        int yMin = (int)Math.Floor(view_rect.yMin);
        int xMax = (int)Math.Ceiling(view_rect.xMax);
        int yMax = (int)Math.Ceiling(view_rect.yMax);


        for (int x = xMin; x <= xMax; x++) {
            for (int y = yMin; y <= yMax; y++) {
                Vector3Int tile_loc = new Vector3Int(x, y, 0);
                if (!tilemap.GetTile(tile_loc)) {
                    tilemap.SetTile(new Vector3Int(x, y, 0), off_tile);
                }
            }
        }
    }

    public void RandomPopulateGridAtView(Rect view_rect) {
        RectOffset padding = new RectOffset(3, 3, 3, 3);
        view_rect = padding.Add(view_rect);

        int xMin = (int)Math.Floor(view_rect.xMin);
        int yMin = (int)Math.Floor(view_rect.yMin);
        int xMax = (int)Math.Ceiling(view_rect.xMax);
        int yMax = (int)Math.Ceiling(view_rect.yMax);


        for (int x = xMin; x <= xMax; x++) {
            for (int y = yMin; y <= yMax; y++) {
                Vector3Int tile_loc = new Vector3Int(x, y, 0);

                if (!tilemap.HasTile(tile_loc)) {
                    TileBase tile_type = off_tile;
                    TM_Symbol tile_symbol = RandomSymbol.Get();
                    switch (tile_symbol) {
                        case TM_Symbol.OFF:
                            tile_type = off_tile;
                            break;
                        case TM_Symbol.ON:
                            tile_type = on_tile;
                            break;
                    }

                    tilemap.SetTile(tile_loc, tile_type);
                }
            }
        }
    }

    // Create a new on tile at the given location
    public void PopulateGridLocation(Vector3Int loc) {
        tilemap.SetTile(loc, on_tile);
    }

    public void FlipTileAtWorldPos(Vector3 pos) {
        Vector3Int tile_pos = tilemap.WorldToCell(pos);
        FlipTileAtTilePos(tile_pos);
    }

    public void FlipTileAtTilePos(Vector3Int tile_pos) {
        EnsureTileExists(tile_pos);
        TileBase tgt_tile = tilemap.GetTile(tile_pos);
        if (tgt_tile == on_tile) {
            tilemap.SetTile(tile_pos, off_tile);
        } else if (tgt_tile == off_tile) {
            tilemap.SetTile(tile_pos, on_tile);
        }
    }

    public TM_Symbol GetTileSymbol(Vector3Int tile_pos) {
        EnsureTileExists(tile_pos);
        TileBase tgt_tile = tilemap.GetTile(tile_pos);
        TM_Symbol symbol = 0;
        if (tgt_tile == on_tile) {
            symbol = TM_Symbol.ON;
        } else if (tgt_tile == off_tile) {
            symbol = TM_Symbol.OFF;
        }
        return symbol;
    }

    public void SetTileSymbol(Vector3Int tile_pos, TM_Symbol symbol) {
        TileBase tile_type = on_tile;
        switch (symbol) {
            case TM_Symbol.OFF:
                tile_type = off_tile;
                break;
            case TM_Symbol.ON:
                tile_type = on_tile;
                break;
        }
        tilemap.SetTile(tile_pos, tile_type);
    }

    private void EnsureTileExists(Vector3Int loc) {
        if (!tilemap.HasTile(loc)) {
            PopulateGridLocation(loc);
        }
    }
}
