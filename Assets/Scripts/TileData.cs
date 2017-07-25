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
public class TileFace {
  // From from left to right and bottom to top
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

  public TileFace(TileFace tf) {
    size_ = tf.size_;
    direction_ = tf.direction_;
    id_ = new Bin(tf.id_.ToString());
    d1_ = tf.d1_;
    d2_ = tf.d2_;
  }

  public TileFace(TileFace tf, Direction direction) {
    size_ = tf.size_;
    direction_ = direction;
    id_ = new Bin(tf.id_.ToString());
    CalculateInvolvedDimensions();
  }

  public void SetIdBit(int x, int y, int z, string bit) {
    int index = GetIndex(x, y, z);
    id_.SetBit(index, bit);
  }

  public string GetIdBit(int x, int y, int z) {
    int index = GetIndex(x, y, z);
    return id_.GetBit(index);
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
    int index = -1;
    switch (direction_) {
      case Direction.NORTH:
      case Direction.WEST:
        index = (size_ - 1 - d1)  + d2 * size_;
        break;
      case Direction.SOUTH:
      case Direction.EAST:
      case Direction.TOP:
      case Direction.BOTTOM:
        index = d1 + d2 * size_;
        break;
    }
    int reversed_index = id_.Length() - 1 - index;
    return reversed_index;
  }
}


public struct SymmetricTile {
  public TileFace[] faces;
  public Vector3 rotation;
}



/// <summary>
/// Data holder for Block prefabs
/// </summary>
public class TileData : MonoBehaviour {
  public int size_;
  public float probability_;
  public SymmetryType symmetry_;
  public TileFace[] faces_;

  public void Initialize() {
    faces_ = new TileFace[6];
    faces_[(int)Direction.TOP] = new TileFace(Direction.TOP, size_);
    faces_[(int)Direction.NORTH] = new TileFace(Direction.NORTH, size_);
    faces_[(int)Direction.EAST] = new TileFace(Direction.EAST, size_);
    faces_[(int)Direction.BOTTOM] = new TileFace(Direction.BOTTOM, size_);
    faces_[(int)Direction.SOUTH] = new TileFace(Direction.SOUTH, size_);
    faces_[(int)Direction.WEST] = new TileFace(Direction.WEST, size_);

    CalculateFaces();
  }

  public SymmetricTile[] GenerateSymmetrics() {
    SymmetricTile[] symmetric_tiles = null;
    int cardinality = 0;
    switch (symmetry_) {
      case SymmetryType.X:
        cardinality = 0;
        break;
      case SymmetryType.L:
        cardinality = 3;
        break;
      case SymmetryType.T:
        cardinality = 3;
        break;
      case SymmetryType.I:
        cardinality = 1;
        break;
      case SymmetryType.D:
        cardinality = 1;
        break;
    }

    symmetric_tiles = new SymmetricTile[cardinality];

    for (int i = 0; i < symmetric_tiles.Length; ++i) {
      int rotation = 90 * (i + 1); //(360 / (symmetric_tiles.Length + 1)) * (i + 1);
      symmetric_tiles[i].faces = GenerateSymmetricTile(rotation);
      symmetric_tiles[i].rotation = new Vector3(0, rotation, 0);
    }

    return symmetric_tiles;
  }

  public TileFace[] GenerateSymmetricTile(int degrees) {
    TileFace[] rotated_faces = new TileFace[6];

    int rotation_steps = Mathf.Abs(degrees / 90);

    Direction rotated_north = (Direction)(((int)Direction.NORTH + rotation_steps) % 4);
    Direction rotated_east = (Direction)(((int)Direction.EAST + rotation_steps) % 4);
    Direction rotated_south = (Direction)(((int)Direction.SOUTH + rotation_steps) % 4);
    Direction rotated_west = (Direction)(((int)Direction.WEST + rotation_steps) % 4);

    rotated_faces[(int)Direction.NORTH] = new TileFace(faces_[(int)rotated_north], Direction.NORTH);
    rotated_faces[(int)Direction.EAST] = new TileFace(faces_[(int)rotated_east], Direction.EAST);
    rotated_faces[(int)Direction.SOUTH] = new TileFace(faces_[(int)rotated_south], Direction.SOUTH);
    rotated_faces[(int)Direction.WEST] = new TileFace(faces_[(int)rotated_west], Direction.WEST);

    // Rotate perpendicular faces
    TileFace top_face = new TileFace(faces_[(int)Direction.TOP]);
    TileFace bottom_face = new TileFace(faces_[(int)Direction.BOTTOM]);

    rotated_faces[(int)Direction.TOP] = RotatePerpendicularFace(top_face, rotation_steps);
    rotated_faces[(int)Direction.BOTTOM] = RotatePerpendicularFace(bottom_face, rotation_steps);

    return rotated_faces;
  }

  private TileFace RotatePerpendicularFace(TileFace face_to_rotate, int rotation_steps) {
    TileFace rotated_face = new TileFace(face_to_rotate);
    int layer_count = (size_ / 2) + 1;

    for (int layer = 0; layer < layer_count; ++layer) {
      int first = layer;
      int last = size_ - first - 1;

      for (int i = first; i < last; ++i) {
        int offset = i - first;

        string[] elements = new string[4];
        // top
        elements[0] = face_to_rotate.GetIdBit(last, 0, last - offset);
        // right
        elements[1] = face_to_rotate.GetIdBit(last - offset, 0, first);
        // bottom
        elements[2] = face_to_rotate.GetIdBit(first, 0, i);
        // left
        elements[3] = face_to_rotate.GetIdBit(i, 0, last);

        // top
        rotated_face.SetIdBit(last, 0, last - offset, elements[rotation_steps]);
        // right
        rotated_face.SetIdBit(last - offset, 0, first, elements[(rotation_steps + 1) % 4]);
        //bottom
        rotated_face.SetIdBit(first, 0, i, elements[(rotation_steps + 2) % 4]);
        // left
        rotated_face.SetIdBit(i, 0, last, elements[(rotation_steps + 3) % 4]);
      }
    }
    return rotated_face;
  }

  /// <summary>
  /// Calculate Tile faces ids (based on tile positions and constraints)
  /// </summary>
  private void CalculateFaces() {
    for (int y = 0; y < size_; ++y) {
      Transform xz_plane = transform.GetChild(y);
      for (int z = 0; z < size_; ++z) {
        Transform x_line = xz_plane.GetChild(z);
        for (int x = 0; x < size_; ++x) {
          // TODO better existence check - this one is prone to fail
          Transform element = x_line.GetChild(x);
          string bit = element.gameObject.GetComponent<MeshRenderer>().enabled ? "1" : "0";
          List<TileFace> involved_faces = GetInvolvedFaces(x, y, z);
          foreach (TileFace face in involved_faces) {
              face.SetIdBit(x, y, z, bit);
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
      faces.Add(faces_[(int)Direction.SOUTH]);
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
