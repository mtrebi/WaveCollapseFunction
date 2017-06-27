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
  }

  private Bin GetBlockBit(Direction direction) {
    int bit_0 = -1,
        bit_1 = -1,
        bit_2 = -1,
        bit_3 = -1;

    switch (direction) {
      case Direction.TOP:
        bit_0 = 4;
        bit_1 = 5;
        bit_2 = 6;
        bit_3 = 7;
        break;
      case Direction.NORTH:
        bit_0 = 3;
        bit_1 = 2;
        bit_2 = 6;
        bit_3 = 7;
        //high_bit = 3;
        //low_bit = 2;
        break;
      case Direction.EAST:
        bit_0 = 1;
        bit_1 = 2;
        bit_2 = 6;
        bit_3 = 5;
        //high_bit = 2;
        //low_bit = 1;
        break;
      case Direction.BOTTOM:
        bit_0 = 0;
        bit_1 = 1;
        bit_2 = 2;
        bit_3 = 3;
        break;
      case Direction.SOUTH:
        bit_0 = 0;
        bit_1 = 1;
        bit_2 = 5;
        bit_3 = 4;
        //high_bit = 0;
        //low_bit = 1;
        break;
      case Direction.WEST:
        bit_0 = 0;
        bit_1 = 3;
        bit_2 = 7;
        bit_3 = 4;
        //high_bit = 3;
        //low_bit = 0;
        break;
    }

    string id = shape_id_.GetBit(bit_3)
                + shape_id_.GetBit(bit_2)
                + shape_id_.GetBit(bit_1)
                + shape_id_.GetBit(bit_0);

    return new Bin(id, 4);
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
