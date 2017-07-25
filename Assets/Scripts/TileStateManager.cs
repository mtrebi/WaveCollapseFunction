﻿using System.Collections;
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
      if (tile_prefab != null) {
        TileData tile_data = tile_prefab.GetComponent<TileData>();
        tile_data.Initialize();

        AddState(new TileState(tile_prefab, tile_data.faces_, tile_data.probability_));

        SymmetricTile[] symmetric_tiles = tile_data.GenerateSymmetrics();
        foreach (SymmetricTile symmetric_tile in symmetric_tiles) {
          TileState tile_state = new TileState(tile_prefab, symmetric_tile.faces, tile_data.probability_, symmetric_tile.rotation);
          AddState(tile_state);
        }
      }else {
        Debug.LogWarning("Null prefab in Tile State Manager");
      }

    }
  }

  private void AddState(TileState tile_state) {
    TileState existing_state = tile_states_.Find(x => x.Equals(tile_state));

    if (existing_state == null) {
      States.Add(tile_state);
    } else {
      if (tile_state.Prefab.name.Equals(existing_state.Prefab.name)) {
        // An automatic symmetric tile ends up being equals to its original tile
        Debug.LogWarning("Duplicated Tile State in symmetric tile" + tile_state.Prefab.name);
      }
      else {
        // Two different prefabs have the same model
        Debug.LogWarning("Duplicated Tile State with names " + tile_state.Prefab.name + " and " + existing_state.Prefab.name);
      }


    }
  }
}
