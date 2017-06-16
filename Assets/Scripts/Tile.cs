using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
  // Position
  private int x_, 
              y_;

  // TileState
  private List<TileState> available_states_;
  private TileState final_state_;

  // Entropy
  private float last_entropy_;
  private bool update_entropy_;
  private float total_probability_;

  /// <summary>
  /// Default constructor - unobserved state - all patterns set to true
  /// </summary>
  public Tile(int x, int y, TileState[] all_states) {
    x_ = x;
    y_ = y;
    total_probability_ = 1.0f;

    update_entropy_ = true;
    final_state_ = null;
    available_states_ = new List<TileState>(all_states);
  }

  /// <summary>
  /// Get final state of the Tile after collapsing
  /// </summary>
  /// <returns> The state of the Tile after collapsing </returns>
  public TileState GetTileState() {
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
      float probability = available_state.GetProbability() + Random.Range(0, 0.025f);
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
}
