using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCFGenerator : MonoBehaviour {
  public int width_,
             height_;
  public bool random_start_;
  public GameObject tile_state_manager_object_;

  private Tile[,] wave_;
  private bool[,] wave_changed_;

  private bool finished_ = false;
  private bool first_iteration_ = true;

  // Use this for initialization
  void Awake() {

  }

  private void Start() {
    InitializeWave();
  }



  // Update is called once per frame
  void Update() {
    if (!finished_) {
      GenerateWave();
      RenderWave();
    }
  }

  private void InitializeWave() {
    wave_ = new Tile[width_, height_];
    wave_changed_ = new bool[width_, height_];
    for (int x = 0; x < width_; ++x) {
      for (int z = 0; z < height_; ++z) {
        wave_[x, z] = TileFactory.Instance.CreateDefaultTile(this.transform, x, z, new List<TileState>(tile_state_manager_object_.GetComponent<TileStateManager>().tile_states_));
        wave_changed_[x, z] = true;
      }
    }
  }

  private void GenerateWave() {
    Tile min_entropy_tile = Observe();

    if (min_entropy_tile == null) {
      Debug.Log("Stopping...");
      finished_ = true;
      return;
    }

    min_entropy_tile.Collapse();
    Propagate(min_entropy_tile);
  }
  
  private void RenderWave() {
    foreach (Tile tile in wave_) {
      if (wave_changed_[tile.X, tile.Z]) {
        tile.Render(this.gameObject.transform);
        wave_changed_[tile.X, tile.Z] = false;
      }
    }
  }

  /// <summary>
  /// Observation step of WCF. Searches for the tile with lowest entropy
  /// </summary>
  /// <returns> Returns the tile with the lowest entropy that has not been collapsed yet</returns>
  private Tile Observe() {
    if (first_iteration_) {
      first_iteration_ = false;
      if (random_start_) {
        return wave_[Random.Range(0, width_), Random.Range(0, height_)];
      }
      return wave_[0, 0];
    }      

    Tile min_entropy_tile = null;
    float min_entropy = float.MaxValue;

    for (int x = 0; x < width_; ++x) {
      for (int y = 0; y < height_; ++y) {
        Tile tile = wave_[x, y];
        if (!tile.Collapsed()) {
          float current_entropy = tile.GetEntropy();

          if (current_entropy < min_entropy) {
            min_entropy_tile = tile;
            min_entropy = current_entropy;
          }
        }

      }
    }
    return min_entropy_tile;
  }

  private void Propagate(Tile tile_changed) {
    Stack<Tile> remaining_tiles = new Stack<Tile>();

    remaining_tiles.Push(tile_changed);

    while (remaining_tiles.Count != 0) {
      Tile current_tile = remaining_tiles.Pop();
      Tile[] neighbors = GetNeighbors(current_tile);

      wave_changed_[current_tile.X, current_tile.Z] = true;

      foreach (Tile neighbor in neighbors) {
        if (!neighbor.Collapsed() 
          && neighbor.UpdateAvailableStates(current_tile, GetDirection(neighbor, current_tile))) {
          remaining_tiles.Push(neighbor);
        }
      }

    }
  }


  private Direction GetDirection (Tile tile, Tile neighbor) {
    if (tile.X > neighbor.X) {
      return Direction.NORTH;
    }

    if (tile.Z < neighbor.Z) {
      return Direction.EAST;
    }

    if (tile.X < neighbor.X) {
      return Direction.SOUTH;
    }

    if (tile.Z > neighbor.Z) {
      return Direction.WEST;
    }

    return Direction.NORTH;
  }

  /// <summary>
  /// Get neighbors of a given tile
  /// </summary>
  /// <param name="tile"></param>
  /// <returns> An array that contains the neighbor tiles </returns>
  private Tile[] GetNeighbors(Tile tile) {
    List<Tile> neighbors = new List<Tile>();
    for (int x = tile.X - 1; x <= tile.X + 1; ++x) {
      for (int y = tile.Z - 1; y <= tile.Z + 1; ++y) {
        if (((x == tile.X && y != tile.Z) || (x != tile.X && y == tile.Z))
            && (x >= 0 && x < width_)
            && (y >= 0 && y < height_)
          ) {
          neighbors.Add(wave_[x, y]);
        }
      }
    }
    return neighbors.ToArray();
  }
}


