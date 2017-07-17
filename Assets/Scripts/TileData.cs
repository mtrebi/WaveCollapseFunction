using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SymmetryType {
  X, 
  T, 
  I,
  L,
  D,// DASH
}

[System.Serializable]
public struct Variation {
  public string id;
  public SymmetryType symmetry_;
}

[System.Serializable]
public class TileFace {
  public Bin id_;
  public Direction direction_;
  public int size_;

  public Dimension d1_, d2_;

  public enum Dimension {
    X,
    Y,
    Z
  };

  public TileFace(Direction direction, int size) {
    size_ = size;
    direction_ = direction;
    id_ = new Bin(size_ * size_);

    CalculateInvolvedDimensions();
  }

  public void UpdateId(int x, int y, int z, string bit) {
    int index = GetIndex(x, y, z);
    id_.SetBit(index, bit);
  }

  private void CalculateInvolvedDimensions() {
    switch (direction_) {
      case Direction.TOP:
      case Direction.BOTTOM:
        d1_ = Dimension.X;
        d2_ = Dimension.Z;
        break;
      case Direction.NORTH:
      case Direction.SOUTH:
        d1_ = Dimension.Z;
        d2_ = Dimension.Y;
        break;
      case Direction.EAST:
      case Direction.WEST:
        d1_ = Dimension.X;
        d2_ = Dimension.Y;
        break;
    }
  }

  private int GetIndex(int x, int y, int z) {
    int d1 = -1, 
        d2 = -1;

    switch (d1_) {
      case Dimension.X:
        d1 = x;
        break;
      case Dimension.Y:
        d1 = y;
        break;
      case Dimension.Z:
        d1 = z;
        break;
    }

    switch (d2_) {
      case Dimension.X:
        d2 = x;
        break;
      case Dimension.Y:
        d2 = y;
        break;
      case Dimension.Z:
        d2 = z;
        break;
    }
    return GetIndex(d1, d2);
  }

  private int GetIndex(int d1, int d2) {
    return d1 + d2 * size_;
  }
}

/// <summary>
/// Data holder for Block prefabs
/// </summary>
public class TileData : MonoBehaviour {
  public int size_;

  public float probability_;
  public string id_;
  public TileFace[] faces_;
  public Variation symmetry_type_;

  private void Start() {
    faces_ = new TileFace[6];
    faces_[(int)Direction.TOP] = new TileFace(Direction.TOP, size_);
    faces_[(int)Direction.NORTH] = new TileFace(Direction.NORTH, size_);
    faces_[(int)Direction.EAST] = new TileFace(Direction.EAST, size_);
    faces_[(int)Direction.BOTTOM] = new TileFace(Direction.BOTTOM, size_);
    faces_[(int)Direction.SOUTH] = new TileFace(Direction.SOUTH, size_);
    faces_[(int)Direction.WEST] = new TileFace(Direction.WEST, size_);

    CalculateId();
  }

  /// <summary>
  /// Calculate Block Id (based on tile positions and constraints)
  /// </summary>
  private void CalculateId() {
    id_ = new string(' ', size_ * size_ * size_);
    for (int y = 0; y < size_; ++y) {
      Transform xz_plane = transform.GetChild(y);
      for (int z = 0; z < size_; ++z) {
        Transform x_line = xz_plane.GetChild(z);
        for (int x = 0; x < size_; ++x) {
          // TODO better existence check
          Transform element = x_line.GetChild(x);
          string bit = element.gameObject.activeInHierarchy ? "1" : "0";
          List<TileFace> involved_faces = GetInvolvedFaces(x, y, z);
          foreach (TileFace face in involved_faces) {
            face.UpdateId(x, y, z, bit);
          }
        }
      }
    }
  }

  /// <summary>
  /// Get which faces are affected by a semi-block with the given indices
  /// </summary>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <param name="z"></param>
  /// <returns>Faces involved in the connection with the given semi-block </returns>
  private List<TileFace> GetInvolvedFaces(int x, int y, int z) {
    List<TileFace> faces = new List<TileFace>();
    if (x == 0) {
      faces.Add(faces_[(int) Direction.SOUTH]);
    }

    if (x == size_ - 1) {
      faces.Add(faces_[(int)Direction.NORTH]);
    }

    if (y == 0) {
      faces.Add(faces_[(int)Direction.BOTTOM]);
    }

    if (y == size_ - 1) {
      faces.Add(faces_[(int)Direction.TOP]);
    }

    if (z == 0) {
      faces.Add(faces_[(int)Direction.WEST]);
    }

    if (z == size_ - 1) {
      faces.Add(faces_[(int)Direction.EAST]);
    }

    return faces;
  }
}
