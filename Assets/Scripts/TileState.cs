using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileState {
  public Texture2D texture_;
  public float probability_;
  public List<TileConstraint> constraints_;

  /* TODO reflections / rotations
  public bool enable_reflections;
  public bool enable_rotations;
  */


  public TileState(Texture2D texture, float probability) {
    texture_ = texture;
    probability_ = probability;
    constraints_ = new List<TileConstraint>();
  }

  public void AddConstraint(Direction direction, TileConstraintId id) {
    constraints_.Add(new TileConstraint(direction, id));
  }

  public TileConstraintId GetConstraint(Direction direction) {
    foreach (TileConstraint tc in constraints_) {
      if (tc.direction_ == direction) {
        return tc.id_;
      }
    }
    return null;
  }

  public float GetProbability() {
    return probability_;
  }

}
