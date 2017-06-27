using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
  TOP,
  NORTH,
  EAST,
  BOTTOM,
  SOUTH,
  WEST
}

static class DirectionMethods {
  public static Direction Opposite(this Direction direction) {
    return (int)direction < 3 ? direction + 3 : direction - 3;
  }
}

public class TileState {
  public Bin shape_id_;
  public GameObject prefab_;
  public Quaternion prefab_orientation_;
  public Vector3 base_rotation_ = new Vector3(0, 0, 0);

  public float probability_;

  private Bin[] connections_;
  

  public TileState(Bin shape_id, GameObject prefab, float probability) {
    shape_id_ = shape_id;
    prefab_ = prefab;
    prefab_orientation_ = Quaternion.Euler(base_rotation_);
    probability_ = probability;
    connections_= CalculateConnections();
  }

  public TileState(Bin shape_id, GameObject prefab, float probability, Vector3 euler_rotation) {
    shape_id_ = shape_id;
    prefab_ = prefab;
    prefab_orientation_ = Quaternion.Euler(base_rotation_ + euler_rotation);
    probability_ = probability;
    connections_ = CalculateConnections();
  }

  private Bin[] CalculateConnections() {
    Bin[] connections = new Bin[6];
    //connections[0] = GetBlockBit(Direction.TOP);
    connections[1] = GetBlockBit(Direction.NORTH);
    connections[2] = GetBlockBit(Direction.EAST);
    //connections[3] = GetBlockBit(Direction.BOTTOM);
    connections[4] = GetBlockBit(Direction.SOUTH);
    connections[5] = GetBlockBit(Direction.WEST);

    return connections;
  }

  public Bin GetConnection(Direction connection) {
    return connections_[(int)connection];
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
    return this.GetConnection(direction).Equals(other.GetConnection(direction.Opposite()));
    //return this.GetBlockBit(direction).Equals(other.GetBlockBit(direction.Opposite()));
  }

  private Bin GetBlockBit(Direction direction) {
    int high_bit = -1, 
        low_bit = -1;
    switch (direction) {
      case Direction.NORTH:
        high_bit = 3;
        low_bit = 2;
        break;
      case Direction.EAST:
        high_bit = 2;
        low_bit = 1;
        break;
      case Direction.SOUTH:
        high_bit = 0;
        low_bit = 1;
        break;
      case Direction.WEST:
        high_bit = 3;
        low_bit = 0;
        break;
    }

    Bin block = new Bin("00", 2);
    block.SetBit(1, shape_id_.GetBit(high_bit));
    block.SetBit(0, shape_id_.GetBit(low_bit));
    return block;
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
}
