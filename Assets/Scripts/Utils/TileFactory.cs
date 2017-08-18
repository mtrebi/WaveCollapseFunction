using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour {
  protected static TileFactory instance_ = null;
  public GameObject placeholder_prefab;

  public static TileFactory Instance {
    get {
      return instance_;
    }
  }

  void Start() {
    instance_ = this;
  }
 
  /// <summary>
  /// Instantiates a Tile with a Placeholer child
  /// </summary>
  /// <param name="x"> X Position of the tile in the grid</param>
  /// <param name="z"> Z Position of the tile in the grid</param>
  /// <param name="possible_states"> List of states that tile can take</param>
  /// <returns> Instantiated Tile </returns>
  public Tile CreateDefaultTile(Transform parent, int x, int y, int z, List<TileModel> available_states) {
    Tile tile = Object.Instantiate(placeholder_prefab, new Vector3(x, y, z), Quaternion.identity).GetComponent<Tile>();
    tile.transform.parent = parent;
    tile.Initialize("Tile", x, y, z, available_states);
    return tile;
  }

  public GameObject CreateTileModel(Transform parent, int x, int y, int z, TileModel model) {
    // Do not render emtpy tiles
    if (model.Type == TileType.EMPTY) {
      return null;
    }
    GameObject tile = Object.Instantiate(model.Prefab, new Vector3(x, y, z), model.Orientation);
    tile.transform.parent = parent;
    tile.name = model.Prefab.name;
    return tile;
  }
}
