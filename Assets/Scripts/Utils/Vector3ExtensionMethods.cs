using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3ExtensionMethods {

  /// <summary>
  /// Compare two vectors
  /// </summary>
  /// <param name="v1"></param>
  /// <param name="v2"></param>
  /// <returns> Returns -1 if v1 is smaller than v2. 
  ///           Returns 0 if v1 and v2 are equals
  ///           Returns 1 if v1 is greater than v2 
  /// </returns>
  public static int CompareTo(this Vector3 v1, Vector3 v2) {
    int result = 0;

    if (LessThan(v1.x, v2.x)) {
      result = -1;
    }
    else if (GreaterThan(v1.x, v2.x)) {
      result = 1;
    }
    else { // Same x
      if (LessThan(v1.y, v2.y)) {
        result = -1;
      }
      else if (GreaterThan(v1.y, v2.y)) {
        result = 1;
      }
      else { // Same x and y
        if (LessThan(v1.z, v2.z)) {
          result = -1;
        }
        else if (GreaterThan(v1.z, v2.z)) {
          result = 1;
        }
      }
    }
    return result;
  }

  //
  public static Vector3 Round(this Vector3 v, int digits = 3) {
    return new Vector3(
      (float)System.Math.Round(v.x, digits),
      (float)System.Math.Round(v.y, digits),
      (float)System.Math.Round(v.z, digits)
    );
  }

  /// <summary>
  /// Compare to floats with a given epsilon
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <param name="epsilon">Comparing difference allowed</param>
  /// <returns>Returns true if a is equals to b. False otherwise</returns>
  private static bool Equals(float a, float b, float epsilon = 0.001f) {
    return System.Math.Abs(a - b) <= epsilon;
  }

  /// <summary>
  /// Compare to floats with a given epsilon
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <param name="epsilon">Comparing difference allowed</param>
  /// <returns>Returns true if a is smaller than b. False otherwise</returns>
  private static bool LessThan(float a, float b, float epsilon = 0.001f) {
    float diff = Mathf.Abs(a - b);
    if (diff > epsilon) {
      return a < b;
    }else {
      return false;
    }
  }

  /// <summary>
  /// Compare to floats with a given epsilon
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <param name="epsilon">Comparing difference allowed</param>
  /// <returns>Returns true if a is greater than b. False otherwise</returns>
  private static bool GreaterThan(float a, float b, float epsilon = 0.001f) {
    float diff = Mathf.Abs(a - b);
    if (diff > epsilon) {
      return a > b;
    }
    else {
      return false;
    }
  }
}
