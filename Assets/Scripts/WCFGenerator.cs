using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Different size tiles!
// TODO: Backtracking

public enum ProgramState {
  INIT,
  RUNNING,
  STOPPED,
  FAILED,
  FINISHED
}

public class WCFGenerator : MonoBehaviour {
  public int width_,
             height_,
             depth_;

  public float randomness_min = 0,
               randomness_max = 0;


  public bool random_start_;
  public GameObject model_manager_object_;

  private Tile[,,] wave_ = null;
  private bool[,,] wave_changed_ = null;

  private ProgramState program_state_ = ProgramState.INIT;
  private bool first_iteration_ = true;

  public ProgramState ProgramState {
    get {
      return program_state_;
    }

    set {
      program_state_ = value;
    }
  }

  public Tile[,,] Wave {
    get {
      return wave_;
    }
  }

  // Use this for initialization
  void Awake() {

  }

  private void Start() {
    InitializeWave();
    List<TileModel> models = model_manager_object_.GetComponent<TileModelManager>().TileModels;
  }

  // Update is called once per frame
  
  void Update() {
    switch (program_state_) {
      case ProgramState.INIT:
        InitializeWave();
        program_state_ = ProgramState.RUNNING;
        break;
      case ProgramState.RUNNING:
        GenerateWave();
        RenderWave();
        break;
      case ProgramState.STOPPED:
        break;
      case ProgramState.FAILED:
        Debug.Log("Program failed. Restarting...");
        program_state_ = ProgramState.INIT;
        break;
      case ProgramState.FINISHED:
        Debug.Log("Program finished successfully...");
        program_state_ = ProgramState.STOPPED;
        break;
    }
  }

  private void InitializeWave() {
    if (wave_ == null) {
      wave_ = new Tile[width_, height_, depth_];
    }

    if (wave_changed_ == null) {
      wave_changed_ = new bool[width_, height_, depth_];
    }

    List<TileModel> all_models = model_manager_object_.GetComponent<TileModelManager>().TileModels;
    List<TileModel> ground_models = all_models.Where(x => x.Type == TileType.GROUND).ToList();
    List<TileModel> empty_models = all_models.Where(x => x.Type == TileType.EMPTY).ToList();
    List<TileModel> models;

    for (int x = 0; x < width_; ++x) {
      for (int y = 0; y < height_; ++y) {
        for (int z = 0; z < depth_; ++z) {
          if (wave_[x, y, z] != null) {
            // TODO : Factory/ pool, null initialize
            Object.Destroy(wave_[x, y, z].gameObject);
          }

          if (y == 0) {
            models = ground_models;
          }
          else if (y == height_ - 1){
            models = empty_models;
          }else {
            models = all_models;
          }

          wave_[x, y, z] = TileFactory.Instance.CreateDefaultTile(this.transform, x, y, z, new List<TileModel>(models));
          wave_changed_[x, y, z] = true;
        }
      }
    }
  }

  private void GenerateWave() {
    Tile min_entropy_tile = Observe();

    if (min_entropy_tile == null) {
      program_state_ = ProgramState.FINISHED;
      return;
    }

    if (!min_entropy_tile.CanCollapse()) {
      program_state_ = ProgramState.FAILED;
      return;
    }

    Collapse(min_entropy_tile);
    Propagate(min_entropy_tile);
  }
  
  private void RenderWave() {
    foreach (Tile tile in wave_) {
      if (wave_changed_[tile.X, tile.Y, tile.Z]) {
        tile.Render(this.gameObject.transform);
        wave_changed_[tile.X, tile.Y, tile.Z] = false;
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
        return wave_[Random.Range(0, width_), 0, Random.Range(0, depth_)];
      }
      return wave_[0, 0, 0];
    }      

    Tile min_entropy_tile = null;
    float min_entropy = float.MaxValue;

    for (int x = 0; x < width_; ++x) {
      for (int y = 0; y < height_; ++y) {
        for (int z = 0; z < depth_; ++z) {
          Tile tile = wave_[x, y, z];
          if (!tile.Collapsed()) {
            float current_entropy = tile.GetEntropy();

            if (current_entropy < min_entropy) {
              min_entropy_tile = tile;
              min_entropy = current_entropy;
            }
          }
        }
      }
    }

    return min_entropy_tile;
  }

  /// <summary>
  /// Collapse to tile to one final state
  /// </summary>
  public void Collapse(Tile tile) {
    List<TileModel> max_probabilities_models = new List<TileModel>();
    float max_probability = float.MinValue;

    foreach (TileModel available_model in tile.AvailableModels) {
      // TODO better use of probabilities
      float probability = available_model.Probability + Random.Range(randomness_min, randomness_max);

      if (probability >= max_probability) {
        if (probability > max_probability) {
          max_probability = probability;
          max_probabilities_models.Clear();
        }
        max_probabilities_models.Add(available_model);
      }
    }

    int random_index = Random.Range(0, max_probabilities_models.Count);
    tile.Collapse(max_probabilities_models[random_index]);
  }


  private void Propagate(Tile tile_changed) {
    Stack<Tile> remaining_tiles = new Stack<Tile>();

    remaining_tiles.Push(tile_changed);

    while (remaining_tiles.Count != 0) {
      Tile current_tile = remaining_tiles.Pop();
      Tile[] neighbors = GetNeighbors(current_tile);

      wave_changed_[current_tile.X, current_tile.Y, current_tile.Z] = true;

      foreach (Tile neighbor in neighbors) {
        if (!neighbor.Collapsed() 
          && neighbor.UpdateAvailableModels(current_tile, GetNeighborOrientation(neighbor, current_tile))) {
          remaining_tiles.Push(neighbor);
        }
      }
    }
  }

  /// <summary>
  /// Get which face of Tile is connected to Neighbor
  /// </summary>
  /// <param name="tile"></param>
  /// <param name="neighbor"></param>
  /// <returns></returns>
  private FaceOrientation GetNeighborOrientation(Tile tile, Tile neighbor) {
    if (tile.X > neighbor.X) {
      return FaceOrientation.NORTH;
    }
    if (tile.X < neighbor.X) {
      return FaceOrientation.SOUTH;
    }

    if (tile.Y < neighbor.Y) {
      return FaceOrientation.TOP;
    }
    if (tile.Y > neighbor.Y) {
      return FaceOrientation.BOTTOM;
    }

    if (tile.Z < neighbor.Z) {
      return FaceOrientation.EAST;
    }
    if (tile.Z > neighbor.Z) {
      return FaceOrientation.WEST;
    }

    return FaceOrientation.NORTH;
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
        for (int z = tile.Z - 1; z <= tile.Z + 1; ++z) {
          if (((x == tile.X && y == tile.Y && z != tile.Z)         
              ||(x == tile.X && y != tile.Y && z == tile.Z)
              ||(x != tile.X && y == tile.Y && z == tile.Z))
              && (x >= 0 && x < width_)
              && (y >= 0 && y < height_)
              && (z >= 0 && z < depth_)
          ) {
            neighbors.Add(wave_[x, y, z]);
          }
        }
      }
    }
    return neighbors.ToArray();
  }
}