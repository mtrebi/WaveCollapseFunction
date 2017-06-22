using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory : MonoBehaviour {
  protected static TileFactory instance_ = null;
  public GameObject default_prefab;

  public static TileFactory Instance {
    get {
      return instance_;
    }
  }

  void Start() {
    instance_ = this;
  }

  /// <summary>
  /// Instantiates a default tile
  /// </summary>
  /// <param name="x"> X Position of the tile in the grid</param>
  /// <param name="y"> Y Position of the tile in the grid</param>
  /// <param name="possible_states"> List of states that tile can take</param>
  /// <returns> Instantiated Game Object </returns>
  public Tile CreateDefaultTile(int x, int y, List<TileState> possible_states) {
    Tile tile = Object.Instantiate(default_prefab, new Vector3(x, 0, y), Quaternion.identity).GetComponent<Tile>();
    tile.Initialize("DefaultTile", x, y, possible_states);
    return tile;
  }
}
