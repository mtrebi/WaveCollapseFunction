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
        TileData tile_data = tile_prefab.GetComponent<TileData>();
        tile_data.Initialize();
        TileState tile_state = new TileState(tile_data);
        States.Add(tile_state);
        // TODO check symmetry type and generate rotations
      }
    }
  }
}
