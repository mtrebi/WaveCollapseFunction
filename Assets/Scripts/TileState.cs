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

[System.Serializable]
public class TileState {
  private GameObject prefab_;

  [SerializeField] private Bin id_;
  [SerializeField] private Bin[] connections_;
  private Quaternion prefab_orientation_;
  private float probability_;


  public Bin Id {
    get {
      return id_;
    }

    set {
      id_ = value;
    }
  }

  public Quaternion PrefabOrientation {
    get {
      return prefab_orientation_;
    }

    set {
      prefab_orientation_ = value;
    }
  }

  public float Probability {
    get {
      return probability_;
    }

    set {
      probability_ = value;
    }
  }

  public GameObject Prefab {
    get {
      return prefab_;
    }

    set {
      prefab_ = value;
    }
  }

  public TileState(Bin shape_id, GameObject prefab, float probability) {
    Id = shape_id;
    this.Prefab = prefab;
    PrefabOrientation = Quaternion.Euler(new Vector3(0, 0, 0));
    Probability = probability;
    connections_= CalculateConnections();
  }

  public TileState(Bin shape_id, GameObject prefab, float probability, Vector3 euler_rotation) {
    Id = shape_id;
    this.Prefab = prefab;
    PrefabOrientation = Quaternion.Euler(euler_rotation);
    Probability = probability;
    connections_ = CalculateConnections();
  }

  private Bin[] CalculateConnections() {
    Bin[] connections = new Bin[6];
    connections[0] = GetBlockBit(Direction.TOP);
    connections[1] = GetBlockBit(Direction.NORTH);
    connections[2] = GetBlockBit(Direction.EAST);
    connections[3] = GetBlockBit(Direction.BOTTOM);
    connections[4] = GetBlockBit(Direction.SOUTH);
    connections[5] = GetBlockBit(Direction.WEST);

    return connections;
  }

  public Bin Connection(Direction connection) {
    return connections_[(int)connection];
  }

  /// <summary>
  /// Check if two tiles can be connected
  /// </summary>
  /// <param name="other">Tile to check satify condition against</param>
  /// <param name="direction">Direction that joins this tile with other</param>
  /// <returns> True if this tile can be connect with the other tile in the given direction</returns>
  public bool Satisfies(TileState other, Direction direction) {
    return this.Connection(direction).Equals(other.Connection(direction.Opposite()));
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
        break;
      case Direction.EAST:
        bit_0 = 1;
        bit_1 = 2;
        bit_2 = 6;
        bit_3 = 5;
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
        break;
      case Direction.WEST:
        bit_0 = 0;
        bit_1 = 3;
        bit_2 = 7;
        bit_3 = 4;
        break;
    }

    string id = Id.GetBit(bit_3)
                + Id.GetBit(bit_2)
                + Id.GetBit(bit_1)
                + Id.GetBit(bit_0);

    return new Bin(id);
  }

  public override string ToString() {
    return Id.ToString();
  }

  public override bool Equals(object obj) {
    var item = obj as TileState;
    if (item == null) return false;
    return this.Id.Equals(item.Id);
  }

  public override int GetHashCode() {
    return this.Id.GetHashCode();
  }
}
