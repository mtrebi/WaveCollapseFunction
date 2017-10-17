using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type {
  GROUND,
  ROOF,
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


  private void OnEnable() {
    SetType();
  }

  private void SetType() {
    /* TODO
    string n = name;
    if (name.Contains(Type.ROOF.ToString(), System.StringComparison.OrdinalIgnoreCase)) {
      Type = Type.ROOF;
    }else if (name.Contains(Type.GROUND.ToString(), System.StringComparison.OrdinalIgnoreCase)) {
      Type = Type.GROUND;
    }else if (name.Contains(Type.EMPTY.ToString(), System.StringComparison.OrdinalIgnoreCase)) {
      Type = Type.EMPTY;
    }else {
      Type = Type.OTHER;
    }*/
  }
}
