using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu()]
public class TileSO : ScriptableObject
{
   
    public GameObject prefabRef;
    public Tile tile;

    public Connection top;
    public Connection bottom;
    public Connection left;
    public Connection right;

}
[System.Serializable]
public class Connection
{
    public List<TileSO> compatileTiles = new();
}
