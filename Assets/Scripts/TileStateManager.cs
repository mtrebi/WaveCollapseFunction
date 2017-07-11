using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStateManager : MonoBehaviour {
  public GameObject[] tile_prefabs_;

  private List<TileState> tile_states_;

  public List<TileState> States {
    get {
      return tile_states_;
    }

    set {
      tile_states_ = value;
    }
  }


  // Use this for initialization
  void Awake () {
    States = new List<TileState>();

    foreach (GameObject tile_prefab in tile_prefabs_) {
      if (tile_prefab != null) { //TODO OUT
        BlockData data = tile_prefab.GetComponent<BlockData>();
        TileState ts = new TileState(new Bin(data.id), tile_prefab, data.probability);
        States.Add(ts);

        foreach (var variation in data.variations) {
          TileState temp = new TileState(new Bin(variation.id), tile_prefab, data.probability, variation.rotation);
          States.Add(temp);
        }
      }

    }
  }

  private TileState[] GenerateRotations(TileState tile_state) {
    HashSet<TileState> tile_rotations = new HashSet<TileState>();

    for (int i = 1; i < tile_state.Id.Length(); ++i) {
      Bin rotation_id = tile_state.Id.Rotate(i);
      // TODO Check id and model
      if (!rotation_id.Equals(tile_state.Id)) {
        TileState new_state = new TileState(rotation_id, tile_state.Prefab, tile_state.Probability, new Vector3(0, -i * 90, 0));
        tile_rotations.Add(new_state);
      }
    }

    TileState[] tile_rotations_array = new TileState[tile_rotations.Count];
    tile_rotations.CopyTo(tile_rotations_array);

    return tile_rotations_array;
  }

}
