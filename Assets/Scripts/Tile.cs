using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
  // Position
  private int x_, 
              y_;

  // TileState
  private List<TileState> available_states_;
  private TileState final_state_;

  // Entropy
  private float last_entropy_;
  private float max_entropy_;
  private bool update_entropy_;
  private float total_probability_;

  /// <summary>
  /// Initialize tile Game object
  /// </summary>
  /// <param name="name"> name of the object</param>
  /// <param name="x"> X position</param>
  /// <param name="y"> Y position</param>
  /// <param name="possible_states"> possible states that tile may take </param>
  public void Initialize(string name, int x, int y,List<TileState> possible_states) {
    this.gameObject.name = name;

    x_ = x;
    y_ = y;
    total_probability_ = 1.0f;

    update_entropy_ = true;
    final_state_ = null;
    available_states_ = possible_states;
    max_entropy_ = GetEntropy();
  }

  /// <summary>
  /// Get final state of the Tile after collapsing
  /// </summary>
  /// <returns> The state of the Tile after collapsing </returns>
  public TileState GetState() {
    return final_state_;
  }

  public bool Collapsed() {
    return final_state_ != null;
  }

  public int X { get { return x_; } }
  public int Y { get { return y_; } }

  /// <summary>
  /// Calculate entropy of the tile given the available patterns
  /// </summary>
  /// <returns> Returns the entropy </returns>
  public float GetEntropy() {
    if (!update_entropy_) {
      return last_entropy_;
    }

    float entropy = 0;
    foreach (TileState available_state in available_states_) {
      float base_p = available_state.GetProbability();
      float p = base_p / total_probability_;
      entropy += -p * Mathf.Log(p, 2);
    }

    update_entropy_ = false;

    return last_entropy_ = entropy;
  }

  /// <summary>
  /// Collapse to tile to one final state
  /// </summary>
  public void Collapse() {
    float max_probability = float.MinValue;
    TileState max_probability_state = null;

    foreach (TileState available_state in available_states_) {
      // TODO better use of probabilities
      float probability = available_state.GetProbability() + Random.Range(0.0f, 0.55f);
      if (probability > max_probability) {
        max_probability = probability;
        max_probability_state = available_state;
      }
    }

    final_state_ = max_probability_state;
    available_states_.Clear();
    available_states_.Add(final_state_);
  }

  /// <summary>
  /// Updates the available states of this tile based on the neighbor constraints
  /// </summary>
  /// <param name="neighbor"></param>
  /// <returns>True if available states changed</returns>
  public bool UpdateAvailableStates(Tile neighbor, Direction direction) {
    bool changed = false;
    for (int i = available_states_.Count - 1; i >= 0; --i) {
      TileState current_tile_state = available_states_[i];
      bool satisfy_any = false;
      foreach (TileState neighbor_tile_state in neighbor.available_states_) {
        if (current_tile_state.Satisfies(neighbor_tile_state, direction)) {
          satisfy_any = true;
          break;
        }
      }

      if (!satisfy_any) {
        changed = true;
        update_entropy_ = true;
        total_probability_ -= available_states_[i].GetProbability();
        available_states_.RemoveAt(i);
      }
    }

    return changed;
  }

  /// <summary>
  /// Updates the aspect of the tile in the game
  /// </summary>
  /// <param name="parent">Gameobject parent transform </param>
  public void Render(Transform parent) {
    Transform transparent_block = this.transform.GetChild(0);
    if (final_state_ != null) {
      // Render final state
      transparent_block.gameObject.SetActive(false);
      TileFactory.Instance.CreateBlock(this.transform, x_, y_, this.final_state_);
    } else {
      // Render based on entropy
      float scale = Mathf.Lerp(0, 1, last_entropy_ / max_entropy_);
      transparent_block.localScale = new Vector3(scale, scale, scale);
    }
  }

  public override string ToString() {
    return "("+ x_ + ", "+ y_ +") - States: " + available_states_.Count + " --> " + final_state_;
  }
}
