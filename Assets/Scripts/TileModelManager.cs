using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages TileModels to generate instanciable 
/// </summary>
public class TileModelManager : MonoBehaviour {
  public GameObject[] tile_model_prefabs_;
  private List<TileInstance> tile_instances_;

  public List<TileInstance> TileInstances {
    get {
      return tile_instances_;
    }

    set {
      tile_instances_ = value;
    }
  }

  // Use this for initialization
  void Awake() {
    TileInstances = new List<TileInstance>();

    foreach (GameObject tile_prefab in tile_model_prefabs_) {
      if (tile_prefab != null) {
        TileModel tile_model = tile_prefab.GetComponent<TileModel>();
        tile_model.Initialize();

        TileInstance[] instances = tile_model.GetTileInstances();
        tile_instances_.AddRange(instances);
      }
      else {
        Debug.LogWarning("Null prefab in Tile State Manager");
      }
    }
  }
}
