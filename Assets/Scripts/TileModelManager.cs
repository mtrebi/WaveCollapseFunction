using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages TileModels to generate instanciable 
/// </summary>
[System.Serializable]
public class TileModelManager : MonoBehaviour {
  public GameObject[] tile_model_prefabs_;
  [SerializeField] private List<TileModel> tile_models_;

  public List<TileModel> TileModels {
    get {
      return tile_models_;
    }

    set {
      tile_models_ = value;
    }
  }

  // Use this for initialization
  void Awake() {
    TileModels = new List<TileModel>();

    foreach (GameObject tile_prefab in tile_model_prefabs_) {
      if (tile_prefab != null) {
        TileData tile_data = tile_prefab.GetComponent<TileData>();
        TileModel tile_model = new TileModel(tile_prefab, tile_data.Probability, tile_data.Type);
        TileModels.Add(tile_model);

        TileModel[] models = tile_model.GetSymmetricModels(tile_data.Symmetry);
        TileModels.AddRange(models);
      }
      else {
        Debug.LogWarning("Null prefab in Tile Model Manager");
      }
    }
  }
}
