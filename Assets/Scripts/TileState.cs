using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
  NORTH,
  WEST,
  SOUTH,
  EAST,
  TOP,
  BOTTOM
}
/* TODO Delete
static class DirectionMethods {
  public static Direction Opposite(this Direction direction) {
    return (int)direction < 3 ? direction + 3 : direction - 3;
  }
}*/

[System.Serializable]
public class TileState {
  private TileFace[] faces_;
  private GameObject prefab_;
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

  public TileFace[] Faces {
    get {
      return faces_;
    }

    set {
      faces_ = value;
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


  public TileState(GameObject prefab, TileFace[] faces, float probability) {
    Prefab = prefab;
    Faces = faces;
    Probability = probability;
    PrefabOrientation = Quaternion.Euler(Vector3.zero);
  }

  public TileState(GameObject prefab, TileFace[] faces, float probability, Vector3 euler_rotation)
    : this(prefab, faces, probability) {
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
    return this.Faces[(int)direction].Equals(other.Faces[(int)direction]);
  }
}
