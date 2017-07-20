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
      TileData tile_data = tile_prefab.GetComponent<TileData>();
      tile_data.Initialize();

      States.Add(new TileState(tile_prefab, tile_data.faces_, tile_data.probability_));

      SymmetricTile[] symmetric_tiles = tile_data.GenerateSymmetrics();
      foreach(SymmetricTile symmetric_tile in symmetric_tiles) {
        TileState tile_state = new TileState(tile_prefab, symmetric_tile.faces, tile_data.probability_, symmetric_tile.rotation);
        States.Add(tile_state);
      }
    }
  }
}
