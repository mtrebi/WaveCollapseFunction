using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCFGenerator : MonoBehaviour {
  public int width,
             height_;
  public bool random_start_;
  public GameObject tile_state_manager_object_;

  private Tile[,] wave_;

  private bool finished_ = false;
  private bool first_iteration_ = true;

  // Use this for initialization
  void Awake() {

  }

  private void Start() {
    InitWave();
  }



  // Update is called once per frame
  void Update() {
    if (!finished_) {
      GenerateTileMap();
    }
  }

  private void InitWave() {
    wave_ = new Tile[width, height_];
    for (int x = 0; x < width; ++x) {
      for (int y = 0; y < height_; ++y) {
        wave_[x, y] = new Tile(x, y, new List<TileState>(tile_state_manager_object_.GetComponent<TileStateManager>().tile_states_));
      }
    }
  }

  private void GenerateTileMap() {
    Tile min_entropy_tile = Observe();

    if (min_entropy_tile == null) {
      Debug.Log("Stopping...");
      finished_ = true;
      return;
    }
    min_entropy_tile.Collapse();
    Draw(min_entropy_tile);
    Propagate(min_entropy_tile);
  }

  private void Draw(Tile tile) {
    TileState state = tile.GetTileState();

    if (state == null) {
      return;
    }

    GameObject new_tile = Instantiate(tile.GetTileState().prefab_, new Vector3(tile.X, 0, tile.Y), tile.GetTileState().prefab_orientation_);
    new_tile.transform.parent = this.gameObject.transform;
    new_tile.name = "Tile_" + tile.GetTileState().shape_id_.ToString();
  }

  private Tile Observe() {
    if (first_iteration_) {
      first_iteration_ = false;
      if (random_start_) {
        return wave_[Random.Range(0, width), Random.Range(0, height_)];
      }
      return wave_[0, 0];
    }      

    Tile min_entropy_tile = null;
    float min_entropy = float.MaxValue;

    for (int x = 0; x < width; ++x) {
      for (int y = 0; y < height_; ++y) {
        Tile tile = wave_[x, y];
        if (tile.GetTileState() == null) {
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

    if (tile.Y < neighbor.Y) {
      return Direction.EAST;
    }

    if (tile.X < neighbor.X) {
      return Direction.SOUTH;
    }

    if (tile.Y > neighbor.Y) {
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
      for (int y = tile.Y - 1; y <= tile.Y + 1; ++y) {
        if (((x == tile.X && y != tile.Y) || (x != tile.X && y == tile.Y))
            && (x >= 0 && x < width)
            && (y >= 0 && y < height_)
          ) {
          neighbors.Add(wave_[x, y]);
        }
      }
    }
    return neighbors.ToArray();
  }
}


