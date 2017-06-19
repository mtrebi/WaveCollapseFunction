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
  public GameObject prefab_;
  public Quaternion prefab_orientation_;
  public Vector3 base_rotation_ = new Vector3(90, -90, -90);

  public float probability_;
  public Bin constraints_id;

  public TileState(Bin shape_id, GameObject prefab, float probability) {
    shape_id_ = shape_id;
    prefab_ = prefab;
    prefab_orientation_ = Quaternion.Euler(base_rotation_);
    probability_ = probability;
    constraints_id = CalculateConstraints();
  }

  public TileState(Bin shape_id, GameObject prefab, float probability, Vector3 euler_rotation) {
    shape_id_ = shape_id;
    prefab_ = prefab;
    prefab_orientation_ = Quaternion.Euler(base_rotation_ + euler_rotation);
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

  public override bool Equals(object obj) {
    var item = obj as TileState;
    if (item == null) return false;
    return this.shape_id_.Equals(item.shape_id_);
  }

  public override int GetHashCode() {
    return this.shape_id_.GetHashCode();
  }

  /// <summary>
  /// Calculate constraints from the TileState shape identifier
  /// </summary>
  private Bin CalculateConstraints() {
    // TODO Actually perform calculation of constraints
    return shape_id_;
  }
}
