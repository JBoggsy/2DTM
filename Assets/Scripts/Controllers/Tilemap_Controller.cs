using System;
using UnityEngine;
using UnityEngine.Tilemaps;

using Constants;

public class Tilemap_Controller : MonoBehaviour
{
    public TileBase OnTile;
    public TileBase OffTile;

    private Tilemap Tilemap;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap = gameObject.GetComponent<Tilemap>();
        Tilemap.SetTile(new Vector3Int(0, 0, 0), OffTile);
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
                if (!Tilemap.GetTile(tile_loc)) {
                    Tilemap.SetTile(new Vector3Int(x, y, 0), OffTile);
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

                if (!Tilemap.HasTile(tile_loc)) {
                    TileBase tile_type = OffTile;
                    TM_Symbol tile_symbol = RandomSymbol.Get();
                    switch (tile_symbol) {
                        case TM_Symbol.OFF:
                            tile_type = OffTile;
                            break;
                        case TM_Symbol.ON:
                            tile_type = OnTile;
                            break;
                    }

                    Tilemap.SetTile(tile_loc, tile_type);
                }
            }
        }
    }

    public void ResetBoard()
    {
        Tilemap.ClearAllTiles();
    }

    // Create a new on tile at the given location
    public void PopulateGridLocation(Vector3Int loc) {
        Tilemap.SetTile(loc, OnTile);
    }

    public void FlipTileAtWorldPos(Vector3 pos) {
        Vector3Int tile_pos = Tilemap.WorldToCell(pos);
        FlipTileAtTilePos(tile_pos);
    }

    public void FlipTileAtTilePos(Vector3Int tile_pos) {
        EnsureTileExists(tile_pos);
        TileBase tgt_tile = Tilemap.GetTile(tile_pos);
        if (tgt_tile == OnTile) {
            Tilemap.SetTile(tile_pos, OffTile);
        } else if (tgt_tile == OffTile) {
            Tilemap.SetTile(tile_pos, OnTile);
        }
    }

    public TM_Symbol GetTileSymbol(Vector3Int tile_pos) {
        EnsureTileExists(tile_pos);
        TileBase tgt_tile = Tilemap.GetTile(tile_pos);
        TM_Symbol symbol = 0;
        if (tgt_tile == OnTile) {
            symbol = TM_Symbol.ON;
        } else if (tgt_tile == OffTile) {
            symbol = TM_Symbol.OFF;
        }
        return symbol;
    }

    public void SetTileSymbol(Vector3Int tile_pos, TM_Symbol symbol) {
        TileBase tile_type = OnTile;
        switch (symbol) {
            case TM_Symbol.OFF:
                tile_type = OffTile;
                break;
            case TM_Symbol.ON:
                tile_type = OnTile;
                break;
        }
        Tilemap.SetTile(tile_pos, tile_type);
    }

    private void EnsureTileExists(Vector3Int loc) {
        if (!Tilemap.HasTile(loc)) {
            PopulateGridLocation(loc);
        }
    }
}
