using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStateManager : MonoBehaviour {
  public GameObject[] tile_prefabs_;
  public List<TileState> tile_states_;
  public bool enable_rotations_;

  // Use this for initialization
  void Start () {
    tile_states_ = new List<TileState>();

    // TODO get id and probability from game object  

    TileState ts_0000 = new TileState(new Bin("0000"), tile_prefabs_[0], 0.2f);
    tile_states_.Add(ts_0000);
    tile_states_.AddRange(GenerateRotations(ts_0000));
    
    TileState ts_1000 = new TileState(new Bin("1000"), tile_prefabs_[1], 0.05f);
    tile_states_.Add(ts_1000);
    tile_states_.AddRange(GenerateRotations(ts_1000));
    
    TileState ts_1100 = new TileState(new Bin("1100"), tile_prefabs_[2], 0.05f);
    tile_states_.Add(ts_1100);
    tile_states_.AddRange(GenerateRotations(ts_1100));
    
    TileState ts_1110 = new TileState(new Bin("1110"), tile_prefabs_[3], 0.05f);
    tile_states_.Add(ts_1110);
    tile_states_.AddRange(GenerateRotations(ts_1110));
    
    TileState ts_1111 = new TileState(new Bin("1111"), tile_prefabs_[4], 0.2f);
    tile_states_.Add(ts_1111);
    tile_states_.AddRange(GenerateRotations(ts_1111));
  }

  private TileState[] GenerateRotations(TileState tile_state) {
    HashSet<TileState> tile_rotations = new HashSet<TileState>();

    for (int i = 1; i < tile_state.shape_id_.Length(); ++i) {
      Bin rotation_id = tile_state.shape_id_.Rotate(i);
      // TODO Check id and model
      // TODO Generate constraint id rotation (cheaper than generate constraint from scratch)
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
