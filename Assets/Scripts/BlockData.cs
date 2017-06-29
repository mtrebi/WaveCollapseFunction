using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Variation {
  public string id;
  public Vector3 rotation;
}

/// <summary>
/// Data holder for Block prefabs
/// </summary>
public class BlockData : MonoBehaviour {
  public float probability;
  public string id;
  public Variation[] variations;
}
