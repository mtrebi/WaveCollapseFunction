using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type {
  GROUND,
  EMPTY,
  OTHER
}

public enum SymmetryType {
  QUAD,
  DOUBLE,
  NONE
}

public class TileData : MonoBehaviour {
  public float Probability;
  public SymmetryType Symmetry;
  public Type Type;
}
