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
  private TileData tile_data_;
  private Quaternion prefab_orientation_;
  private float probability_;

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

  public TileData TileData {
    get {
      return tile_data_;
    }

    set {
      tile_data_ = value;
    }
  }

  public TileState(TileData tile_data) {
    TileData = tile_data;
    Probability = tile_data.probability_;
    PrefabOrientation = Quaternion.Euler(Vector3.zero);
  }

  public TileState(TileData tile_data, Vector3 euler_rotation) {
    TileData = tile_data;
    Probability = tile_data.probability_;
    PrefabOrientation = Quaternion.Euler(euler_rotation);
  }

  /// <summary>
  /// Check if two tiles can be connected
  /// </summary>
  /// <param name="other">Tile to check satify condition against</param>
  /// <param name="direction">Direction that joins this tile with other</param>
  /// <returns> True if this tile can be connect with the other tile in the given direction</returns>
  public bool Satisfies(TileState other, Direction direction) {
    // TODO because of symmetry --| Faces should be modified on TileState
    return this.TileData.faces_[(int)direction].Equals(other.TileData.faces_[(int)direction]);
  }
}
