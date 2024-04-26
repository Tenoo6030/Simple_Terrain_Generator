using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class WaveFunction : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private TileSO missingTile;

    private TileSO[,] grid;
    private List<Vector2Int> offsets = new List<Vector2Int>()
    {
        new Vector2Int(0, 1),   // top
        new Vector2Int(0, -1),  // bottom
        new Vector2Int(1, 0),   // right
        new Vector2Int(-1, 0)   // left
    };
    private List<Vector2Int> toCollapse = new();
    public List<TileSO> tiles = new();


    private void Start()
    {
        grid = new TileSO[width, height];

        CollapseWorld();
    }


    private void CollapseWorld()
    {
        toCollapse.Clear();
        toCollapse.Add(new Vector2Int(width/2,height/2));

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
                                CheckValidity(potentialTile, neighbourTile.bottom.compatileTiles); break;
                            case 1:
                                CheckValidity(potentialTile, neighbourTile.top.compatileTiles); break;
                            case 2:
                                CheckValidity(potentialTile, neighbourTile.left.compatileTiles); break;
                            case 3:
                                CheckValidity(potentialTile, neighbourTile.right.compatileTiles); break;
                        }
                    }
                    else
                    {
                        if (!toCollapse.Contains(neighbour)) toCollapse.Add(neighbour);
                    }
                }
            }

            if (potentialTile.Count < 1)
            {
                grid[x, y] = missingTile;
            }
            else
            {
                grid[x, y] = potentialTile[Random.Range(0, potentialTile.Count)];
            }

            GameObject newTile = Instantiate(grid[x, y].prefabRef, new Vector3(x, y, 0f), Quaternion.identity);

            toCollapse.RemoveAt(0);
        }
    }

    private bool IsInsideGrid(Vector2Int pos)
    {
        if (pos.x > -1 && pos.x < width && pos.y > -1 && pos.y < height)
        {
            return true;
        }
        else
        {
            return false;
        }
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
