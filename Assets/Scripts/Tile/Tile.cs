using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
  // Position
  private int x_,
              y_,
              z_;

  [SerializeField] private TileModel model_;
  [SerializeField] private List<TileModel> available_models_;

  // Entropy
  [SerializeField] private float entropy_;
  private float max_entropy_;
  private float total_probability_;

  public int X { get { return x_; } }
  public int Y { get { return y_; } }
  public int Z { get { return z_; } }

  /// <summary>
  /// Final Model of the Tile that determines its rendered shape
  /// </summary>
  public TileModel Model {
    get {
      return model_;
    }
  }

  /// <summary>
  /// List of available models that could (eventually) be the final model of the Tile
  /// </summary>
  public List<TileModel> AvailableModels {
    get {
      return available_models_;
    }
  }

  /// <summary>
  /// Check if tile can be collapsed into a final tile model
  /// </summary>
  /// <returns>Returns true if tile can collapse. False otherwise</returns>
  public bool CanCollapse() {
    return available_models_.Count != 0;
  }

  /// <summary>
  /// Collapse Tile into a TileModel
  /// </summary>
  /// <param name="model"></param>
  public void Collapse(TileModel model) {
    model_ = model;
    available_models_.Clear();
    available_models_.Add(model_);

    this.transform.FindChild("Placeholder").gameObject.SetActive(false);
    TileFactory.Instance.CreateTileModel(this.transform.FindChild("Model"), x_, y_, z_, this.model_);
  }

  /// <summary>
  /// Check if the Tile already collaped into a final model
  /// </summary>
  /// <returns> True if Tile already collapsed </returns>
  public bool Collapsed() {
    return model_ != null;
  }



  /// <summary>
  /// Initialize tile Game object
  /// </summary>
  /// <param name="name"> name of the object</param>
  /// <param name="x"> X position</param>
  /// <param name="y"> Y position</param>
  /// <param name="z"> Z position</param>
  /// <param name="available_models"> possible models that tile may take </param>
  public void Initialize(string name, int x, int y, int z, List<TileModel> available_models) {
    this.gameObject.name = name;

    x_ = x;
    y_ = y;
    z_ = z;

    model_ = null;
    available_models_ = available_models;
    total_probability_ = TotalProbability();
    max_entropy_ = GetMaxEntropy();
    entropy_ = max_entropy_;
  }

  /// <summary>
  /// Calculate entropy of the tile given the available patterns
  /// </summary>
  /// <returns> Returns the entropy </returns>
  public float GetMaxEntropy() {
    entropy_ = 0;
    foreach (TileModel available_model in available_models_) {
      float p = available_model.Probability / total_probability_;
      entropy_ += -p * Mathf.Log(p, 2);
    }

    return entropy_;
  }

  /// <summary>
  /// Gets entropy of the tile given the available patterns
  /// </summary>
  /// <returns> Returns the entropy </returns>
  public float GetEntropy() {
    return entropy_;
  }

  /// <summary>
  /// Updates the available models of this tile based on the neighbor constraints
  /// </summary>
  /// <param name="neighbor"></param>
  /// <param name="orientation"> Which face of tile is connected with the neighbor tile</param>
  /// <returns>True if available models changed</returns>
  public bool UpdateAvailableModels(Tile neighbor, FaceOrientation orientation) {
    bool changed = false;
    for (int i = available_models_.Count - 1; i >= 0; --i) {
      TileModel current_model = available_models_[i];
      bool satisfy_any = false;
      foreach (TileModel neighbor_model in neighbor.available_models_) {
        if (current_model.Adjacencies.Adjacencies[(int)orientation]
          .Match(neighbor_model.Adjacencies.Adjacencies[(int)orientation.Opposite()])) {
          satisfy_any = true;
          break;
        }
      }

      if (!satisfy_any) {
        changed = true;
        RemoveAvailableModel(i);
      }
    }

    return changed;
  }

  /// <summary>
  /// Updates the aspect of the tile in the game
  /// </summary>
  /// <param name="parent">Gameobject parent transform </param>
  public void Render(Transform parent) {
    if (model_ == null) {
      // Render based on entropy
      Transform placeholder = this.transform.FindChild("Placeholder");

      float scale = max_entropy_ == 0 ? 0.1f : Mathf.Lerp(0.1f, 0.8f, entropy_ / max_entropy_);
      placeholder.localScale = new Vector3(scale, scale, scale);
    }
  }

  public override string ToString() {
    return "("+ x_ + ", " + y_ + ", " + z_ +") - States: " + available_models_.Count + " --> " + model_;
  }

  private void RemoveAvailableModel(int index) {
    float p = available_models_[index].Probability / total_probability_;
    entropy_ -= -p * Mathf.Log(p, 2);
    available_models_.RemoveAt(index);
  }

  private float TotalProbability() {
    float total_probability = 0;
    foreach (TileModel model in available_models_) {
      total_probability += model.Probability;
    }
    return total_probability;
  }
}