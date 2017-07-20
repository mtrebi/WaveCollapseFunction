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
  public Tile CreateTilePlaceholder(Transform parent, int x, int y, int z, List<TileState> possible_states) {
    Tile tile = Object.Instantiate(placeholder_prefab, new Vector3(x, y, z), Quaternion.identity).GetComponent<Tile>();
    tile.transform.parent = parent;
    tile.Initialize("Tile", x, y, z, possible_states);
    return tile;
  }

  /// <summary>
  /// Instantiates a TileData object
  /// </summary>
  /// <param name="parent"> Parent of the new tile</param>
  /// <param name="x"> X Position of the tile in the grid</param>
  /// <param name="y"> Y Position of the tile in the grid</param>
  /// <param name="z"> Z Position of the tile in the grid</param>
  /// <param name="state"> State of the tile (determines prefab_ and rotation)</param>
  /// <returns> The new instantiated tile </returns>
  public TileData CreateTileData(Transform parent, int x, int y, int z, TileState state) {
    TileData tile_data = Object.Instantiate(state.Prefab, new Vector3(x, y, z), state.PrefabOrientation).GetComponent<TileData>();
    tile_data.transform.parent = parent;
    tile_data.name = state.Prefab.name;
    return tile_data;
  }
}
