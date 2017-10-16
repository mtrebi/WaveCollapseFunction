using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Backtracking

public enum ProgramState {
  INIT,
  RUNNING,
  STOPPED,
  FAILED,
  FINISHED
}

public class WCFGenerator : MonoBehaviour {
  private int width_,
             height_,
             depth_;

  private int current_layer_ = 0;

  public float randomness_min = 0,
               randomness_max = 0;

  /// <summary>
  /// Reference to the model manager object that contains all tile models used by the Tile Objects
  /// </summary>
  public GameObject model_manager_object_;

  /// <summary>
  /// Stores each Tile Object that is rendered to create the object
  /// </summary>
  private Tile[,,] wave_ = null;

  /// <summary>
  /// Stores a flag that indicates if the wave_ structure has changed since the last frame
  /// </summary>
  private bool[,,] wave_changed_ = null;

  private ProgramState program_state_ = ProgramState.STOPPED;

  private int[] available_corners_counter_,
                collapsed_corners_counter_;


  // Models
  private List<TileModel> all_models_,
                          ground_models_,
                          empty_models_;

  public int Width {
    get {
      return width_;
    }

    set {
      width_ = value;
    }
  }

  public int Depth {
    get {
      return depth_;
    }

    set {
      depth_ = value;
    }
  }

  public int Height {
    get {
      return height_;
    }

    set {
      height_ = value;
    }
  }

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
    // Initialize models structure
    all_models_ = model_manager_object_.GetComponent<TileModelManager>().TileModels;
    ground_models_ = all_models_.Where(x => x.Type == Type.GROUND || x.Type == Type.EMPTY).ToList();
    empty_models_ = all_models_.Where(x => x.Type == Type.EMPTY).ToList();

    available_corners_counter_ = new int[4];
    collapsed_corners_counter_ = new int[4];
  }

  void Update() {
    // TODO SPEED
    //for(int i = 0; i < 9e7; ++i) {}

    switch (program_state_) {
      case ProgramState.INIT:
        Debug.Log("Starting...");
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
    current_layer_ = 0;
    if (wave_ == null) {
      wave_ = new Tile[width_, height_, depth_];
    }

    if (wave_changed_ == null) {
      wave_changed_ = new bool[width_, height_, depth_];
    }

    for (int x = 0; x < width_; ++x) {
      for (int y = 0; y < height_; ++y) {
        for (int z = 0; z < depth_; ++z) {
          if (wave_[x, y, z] != null) {
            // TODO : Factory/ pool, null initialize
            Object.Destroy(wave_[x, y, z].gameObject);
          }

          List<TileModel> models = GetTileModelsList(x, y, z);
          wave_[x, y, z] = TileFactory.Instance.CreateDefaultTile(this.transform, x, y, z, new List<TileModel>(models));
          wave_changed_[x, y, z] = true;
        }
      }
    }
  }

  public void DestroyWave() {
    for (int x = 0; x < width_; ++x) {
      for (int y = 0; y < height_; ++y) {
        for (int z = 0; z < depth_; ++z) {
          if (wave_ != null && wave_[x, y, z] != null) {
            // TODO : Factory/ pool, null initialize
            Object.Destroy(wave_[x, y, z].gameObject);
          }
        }
      }
    }
    wave_ = null;
    wave_changed_ = null;
  }

  private void GenerateWave() {
    Tile min_entropy_tile = ObserveLayer();
    if (!min_entropy_tile) {
      ChangeLayer();
      min_entropy_tile = ObserveLayer();
    }

    if (min_entropy_tile == null) {
      program_state_ = ProgramState.FINISHED;
      return;
    }

    if (!min_entropy_tile.CanCollapse()) {
      program_state_ = ProgramState.FAILED;
      return;
    }

    Collapse(min_entropy_tile);
    PropagateSideEntropy(min_entropy_tile);
  }
  
  private void ChangeLayer() {
    if (current_layer_ == height_ - 1) {
      return;
    }
    ++current_layer_;
    PropagateFromBottomLayer();
    for (int x = 0; x < width_; ++x) {
      for (int z = 0; z < depth_; ++z) {
        PropagateSideEntropy(wave_[x, current_layer_, z]);
      }
    }
  }

  // Propagate entropy from current layer to top one
  private void PropagateFromBottomLayer() {
    for (int x = 0; x < width_; ++x) {
      for (int z = 0; z < depth_; ++z) {
        Tile tile = wave_[x, current_layer_, z];
        Tile[] bottom_neighbors = GetBottomNeighbors(tile);
        wave_changed_[tile.X, tile.Y, tile.Z] = true;

        foreach (Tile neighbor in bottom_neighbors) {
          if (!tile.Collapsed()) {
            tile.UpdateAvailableModels(neighbor, GetNeighborOrientation(tile, neighbor));
          }
        }
      }
    }
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
  /// Observation step of WCF. Searches for the tile with lowest entropy in the current layer
  /// </summary>
  /// <returns> Returns the tile with the lowest entropy that has not been collapsed yet in the current layer. Returns null if all Tiles of the current layer have collapsed.</returns>
  private Tile ObserveLayer() {
    Tile min_entropy_tile = null;
    float min_entropy = float.MaxValue;

    for (int x = 0; x < width_; ++x) {
      for (int z = 0; z < depth_; ++z) {
        Tile tile = wave_[x, current_layer_, z];
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

  /// <summary>
  /// Collapse to tile to one final state
  /// </summary>
  public void Collapse(Tile tile) {
    List<TileModel> max_probabilities_models = new List<TileModel>();
    float max_probability = float.MinValue;

    foreach (TileModel available_model in tile.AvailableModels) {
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
    TileModel collapse_model = max_probabilities_models[random_index];
    tile.Collapse(collapse_model);
  }


  private void PropagateSideEntropy(Tile tile_changed) {
    Stack<Tile> remaining_tiles = new Stack<Tile>();

    remaining_tiles.Push(tile_changed);

    while (remaining_tiles.Count != 0) {
      Tile current_tile = remaining_tiles.Pop();
      Tile[] neighbors = GetSideNeighbors(current_tile);

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
  /// Get upper neighbors of a given tile
  /// </summary>
  /// <param name="tile"></param>
  /// <returns> An array that contains the upper neighbor tiles </returns>
  private Tile[] GetBottomNeighbors(Tile tile) {
    List<Tile> neighbors = new List<Tile>();
    if (tile.Y - 1 >= 0) {
      neighbors.Add(wave_[tile.X, tile.Y - 1, tile.Z]);
    }

    return neighbors.ToArray();
  }

  /// <summary>
  /// Get side neighbors of a given tile (north, south, east, west)
  /// </summary>
  /// <param name="tile"></param>
  /// <returns> An array that contains the neighbor tiles </returns>
  private Tile[] GetSideNeighbors(Tile tile) {
    List<Tile> neighbors = new List<Tile>();
    for (int x = tile.X - 1; x <= tile.X + 1; ++x) {
      for (int z = tile.Z - 1; z <= tile.Z + 1; ++z) {
        if ((x >= 0 && x < width_) && (z >= 0 && z < depth_) &&
            ((x == tile.X &&  z != tile.Z) || (x != tile.X && z == tile.Z))) {
          neighbors.Add(wave_[x, tile.Y, z]);
        }
      }
    }
    return neighbors.ToArray();
  }


  private List<TileModel> GetTileModelsList(int x, int y, int z) {
    // Outter side
    if (x == 0 || x == width_ - 1 || z == 0 || z == depth_ - 1 || y == height_ -1) {
      return empty_models_;
    }

    // Bottom layer
    if (y == 0) {
      return ground_models_;
    }

    // Rest
    return all_models_; 
  }

  bool CanFinish() {
    return true;
    // MInimo 4
    // Los que sean mayor que 4 se tienen que contrarestar con su counter
    // 1 por cada INV que haya
  }
}