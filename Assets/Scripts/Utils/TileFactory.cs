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
  /// <returns> Instantiated Tile </returns>
  public Tile CreateDefaultTile(Transform parent, int x, int y, List<TileState> possible_states) {
    Tile tile = Object.Instantiate(default_prefab, new Vector3(x, 0, y), Quaternion.identity).GetComponent<Tile>();
    tile.transform.parent = parent;
    tile.Initialize("Tile", x, y, possible_states);
    return tile;
  }

  /// <summary>
  /// Instantiates a block
  /// </summary>
  /// <param name="parent"> Parent of the new block</param>
  /// <param name="x"> X Position of the tile in the grid</param>
  /// <param name="y"> Y Position of the tile in the grid</param>
  /// <param name="state"> State of the block (determines prefab and rotation)</param>
  /// <returns> The new instantiated block </returns>
  public BlockData CreateBlock(Transform parent, int x, int y, TileState state) {
    BlockData block = Object.Instantiate(state.prefab_, new Vector3(x, 0, y), state.prefab_orientation_).GetComponent<BlockData>();
    block.transform.parent = parent;
    block.name = "Block_" + state;
    return block;
  }
}
