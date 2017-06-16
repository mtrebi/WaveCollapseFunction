using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
  NORTH,
  EAST,
  SOUTH,
  WEST
}

static class DirectionMethods {
  public static Direction Opposite(this Direction direction) {
    return (int)direction < 2 ? direction + 2 : direction - 2;
  }
}

public class TileState {
  public Bin shape_id_;
  public Texture2D texture_;
  public float probability_;
  public Bin constraints_id;

  public TileState(Bin shape_id, Texture2D texture, float probability) {
    shape_id_ = shape_id;
    texture_ = texture;
    probability_ = probability;
    constraints_id = CalculateConstraints();
  }

  public float GetProbability() {
    return probability_;
  }

  /// <summary>
  /// Check if two tiles can be connected
  /// </summary>
  /// <param name="other">Tile to check satify condition against</param>
  /// <param name="direction">Direction that joins this tile with other</param>
  /// <returns> True if this tile can be connect with the other tile in the given direction</returns>
  public bool Satisfies(TileState other, Direction direction) {
    return this.shape_id_.GetBit((int) direction) == other.shape_id_.GetBit((int) direction.Opposite());
  }

  public override string ToString() {
    return shape_id_.ToString();
  }


  /// <summary>
  /// Calculate constraints from the TileState shape identifier
  /// </summary>
  private Bin CalculateConstraints() {
    // TODO Actually perform calculation of constraints
    return shape_id_;
  }
}
