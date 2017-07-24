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

static class DirectionMethods {
  public static Direction Opposite(this Direction direction) {
    switch (direction) {
      case Direction.NORTH:
        return Direction.SOUTH;
      case Direction.WEST:
        return Direction.EAST;
      case Direction.SOUTH:
        return Direction.NORTH;
      case Direction.EAST:
        return Direction.WEST;
      case Direction.TOP:
        return Direction.BOTTOM;
      case Direction.BOTTOM:
        return Direction.TOP;
    }
    return (Direction) (-1);
  }
}

[System.Serializable]
public class TileState {
  [SerializeField]
  private Bin id_;
  private TileFace[] faces_;
  private GameObject prefab_;
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
    string id = "";

    foreach (TileFace face in faces) {
      id += face.id_;
    }

    Id = new Bin(id);
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
    TileFace this_face = this.Faces[(int)direction];
    TileFace other_face = other.Faces[(int)direction.Opposite()];


    if (direction == Direction.TOP || direction == Direction.BOTTOM) {
      return this_face.id_.Equals(other_face.id_);
    }

    // Because we always use the same reference system to denote faces
    // We need to reverse it when performing the comparision
    string[] blocked_reversed_id = new string[other_face.size_];
    for (int i = 0; i < blocked_reversed_id.Length; ++i) {
      blocked_reversed_id[i] = other_face.id_.ToString().Substring(i * other_face.size_, other_face.size_);
      char[] arr = blocked_reversed_id[i].ToCharArray();
      System.Array.Reverse(arr);
      blocked_reversed_id[i] = new string(arr);
    }
    Bin aux = new Bin(string.Join("", blocked_reversed_id));

    return this_face.id_.Equals(aux);
  }

  public override bool Equals(object obj) {
    var item = obj as TileState;

    if (item == null) {
      return false;
    }

    return this.Id.Equals(item.Id);
  }

  public override int GetHashCode() {
    return this.Id.GetHashCode();
  }
}
