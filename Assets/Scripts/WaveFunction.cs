using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveFunction : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private TileSO missingTile;
    [SerializeField] private Tilemap map;

    private TileBase visibleTile;
    private TileSO[,] grid;
    private List<Vector2Int> toCollapse = new();
    private List<Vector2Int> offsets = new List<Vector2Int>()
    {
        new Vector2Int(0, 1),   // top
        new Vector2Int(0, -1),  // bottom
        new Vector2Int(1, 0),   // right
        new Vector2Int(-1, 0)   // left
    };

    public List<TileSO> tiles = new();


    private void Start()
    {
        grid = new TileSO[width, height];

        CollapseWorld();
    }

    private void CollapseWorld()
    {
        toCollapse.Clear();
        toCollapse.Add(new Vector2Int(width / 2, height / 2));

        while (toCollapse.Count > 0)
        {
            int x = toCollapse[0].x;
            int y = toCollapse[0].y;

            List<TileSO> potentialTile = new(tiles);

            for (int i = 0; i < offsets.Count; i++)
            {
                Vector2Int neighbour = new Vector2Int(x + offsets[i].x, y + offsets[i].y);

                if (IsInsideGrid(neighbour))
                {
                    TileSO neighbourTile = grid[neighbour.x, neighbour.y];

                    if (neighbourTile != null)
                    {
                        switch (i)
                        {
                            case 0:
                                CheckValidity(potentialTile, neighbourTile.bottom.compatileTiles);
                                break;
                            case 1:
                                CheckValidity(potentialTile, neighbourTile.top.compatileTiles);
                                break;
                            case 2:
                                CheckValidity(potentialTile, neighbourTile.left.compatileTiles);
                                break;
                            case 3:
                                CheckValidity(potentialTile, neighbourTile.right.compatileTiles);
                                break;
                        }
                    }
                    else
                    {
                        if (!toCollapse.Contains(neighbour)) toCollapse.Add(neighbour);
                    }
                }
            }

            grid[x, y] = potentialTile.Count < 1 ? missingTile : GetWeightRandomTile(potentialTile);

            Vector3Int location = new Vector3Int(x, y, 0);
            visibleTile = grid[x, y].tile;
            map.SetTile(location, visibleTile);

            toCollapse.RemoveAt(0);
        }
    }
    private TileSO GetWeightRandomTile(List<TileSO> tiles)
    {
        int sumWeight = 0;

        foreach (TileSO tile in tiles)
        {
            sumWeight += tile.weight;
        }

        int pivot = Random.Range(0, sumWeight);

        foreach (TileSO tile in tiles)
        {
            if (pivot < tile.weight) return tile;

            pivot -= tile.weight;
        }

        return missingTile;
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x > -1 && pos.x < width && pos.y > -1 && pos.y < height;
    }

    private void CheckValidity(List<TileSO> optionList, List<TileSO> validOption)
    {
        for (int i = optionList.Count - 1; i > -1; i--)
        {
            if (!validOption.Contains(optionList[i]))
            {
                optionList.RemoveAt(i);
            }
        }
    }
}
