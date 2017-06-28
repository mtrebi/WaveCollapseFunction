using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStateManager : MonoBehaviour {
  public GameObject[] tile_prefabs_;
  public List<TileState> tile_states_;
  public bool enable_rotations_;

  // Use this for initialization
  void Awake () {
    tile_states_ = new List<TileState>();

    foreach (GameObject tile_prefab in tile_prefabs_) {
      BlockData data = tile_prefab.GetComponent<BlockData>();
      TileState ts = new TileState(new Bin(data.id), tile_prefab, data.probability);
      tile_states_.Add(ts);
      tile_states_.AddRange(GenerateRotations(ts));
    }
  }

  private TileState[] GenerateRotations(TileState tile_state) {
    HashSet<TileState> tile_rotations = new HashSet<TileState>();

    for (int i = 1; i < tile_state.shape_id_.Length(); ++i) {
      Bin rotation_id = tile_state.shape_id_.Rotate(i);
      // TODO Check id and model
      if (!rotation_id.Equals(tile_state.shape_id_)) {
        TileState new_state = new TileState(rotation_id, tile_state.prefab_, tile_state.probability_, new Vector3(0, -i * 90, 0));
        tile_rotations.Add(new_state);
      }
    }

    TileState[] tile_rotations_array = new TileState[tile_rotations.Count];
    tile_rotations.CopyTo(tile_rotations_array);

    return tile_rotations_array;
  }

}
