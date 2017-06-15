using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCFGenerator : MonoBehaviour {
  public int widht_,
             height_;
  public bool random_start_;




  public GameObject tile_state_manager_object_;
  public GameObject tile_prefab_;


  private TileStateManager tile_state_manager_;
  private Tile[,] wave_;

  private bool finished_ = false;
  private bool first_iteration_ = true;

  // Use this for initialization
  void Awake() {
    tile_state_manager_ = tile_state_manager_object_.GetComponent<TileStateManager>();
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
    wave_ = new Tile[widht_, height_];
    for (int x = 0; x < widht_; ++x) {
      for (int y = 0; y < height_; ++y) {
        wave_[x, y] = new Tile(x, y, tile_state_manager_.tile_states_);
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

  // TODO Scale
  private void Draw(Tile tile) {
    TileState state = tile.GetTileState();

    if (state == null) {
      return;
    }

    GameObject new_tile = Instantiate(tile_prefab_, new Vector3(tile.X, tile.Y, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
    new_tile.GetComponent<Renderer>().material.mainTexture = state.texture_;
    new_tile.transform.parent = this.gameObject.transform;
  }

  private Tile Observe() {
    if (first_iteration_) {
      first_iteration_ = false;
      if (random_start_) {
        return wave_[Random.Range(0, widht_), Random.Range(0, height_)];
      }
      return wave_[0, 0];
    }      

    Tile min_entropy_tile = null;
    float min_entropy = float.MaxValue;

    for (int x = 0; x < widht_; ++x) {
      for (int y = 0; y < height_; ++y) {
        Tile tile = wave_[x, y];
        if (tile.GetTileState() == null) {
          float current_entropy = tile.GetEntropy();

          if (current_entropy != 0 && current_entropy < min_entropy) {
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
    if (tile.Y < neighbor.Y) {
      return Direction.NORTH;
    }

    if (tile.X > neighbor.X) {
      return Direction.EAST;
    }

    if (tile.Y > neighbor.Y) {
      return Direction.SOUTH;
    }

    if (tile.X < neighbor.X) {
      return Direction.WEST;
    }

    return Direction.NORTH;
  }

  // TODO> Move to tile
  /// <summary>
  /// Get neighbors of a given tile
  /// </summary>
  /// <param name="tile"></param>
  /// <returns> An array that contains the neighbor tiles </returns>
  private Tile[] GetNeighbors(Tile tile) {
    List<Tile> neighbors = new List<Tile>();
    for (int x = tile.X - 1; x <= tile.X + 1; ++x) {
      for (int y = tile.Y - 1; y <= tile.Y + 1; ++y) {
        bool a = (x == tile.X && y != tile.Y);
        bool b = (x != tile.X && y == tile.Y);
        if (((x == tile.X && y != tile.Y) || (x != tile.X && y == tile.Y))
            && (x >= 0 && x < widht_)
            && (y >= 0 && y < height_)
          ) {
          neighbors.Add(wave_[x, y]);
        }
      }
    }
    return neighbors.ToArray();
  }
}


