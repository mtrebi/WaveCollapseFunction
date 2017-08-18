using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
  GROUND,
  WALL,
  ROOF,
  EMPTY
}

public class TileData : MonoBehaviour {
  public float Probability;
  public SymmetryType Symmetry;
  public TileType Type;
}
