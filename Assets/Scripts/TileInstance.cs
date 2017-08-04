using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInstance {
  private GameObject prefab_;
  private Quaternion prefab_orientation_;
  private float probability_;
  [SerializeField] private TileAdjacencies adjacencies_;


  public TileInstance(GameObject prefab, Quaternion rotation, float probability, TileAdjacencies adjacencies) {
    prefab_ = prefab;
    prefab_orientation_ = rotation;
    probability_ = probability;
    adjacencies_ = adjacencies;
  }


  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
