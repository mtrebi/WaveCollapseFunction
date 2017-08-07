using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mathematics {

  public static Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Vector3 euler) {
    return Quaternion.Euler(euler) * (point - pivot) + pivot;
  }
}
