using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum Mode {
  TEST_MODEL,
  TEST_WAVE
}

public class DebugTool : MonoBehaviour {
  public Mode mode;

  public WCFGenerator Generator;
  public GameObject FaceGizmoPrefab;
  public GameObject TileModelManagerPrefab;


  private GameObject selected_;
  private GameObject[] face_gizmos_;
  private TileModel tile_model_;
  private TileModelManager model_manager_;
  private Tile[,,] testing_tiles_;

  void Start() {
    face_gizmos_ = new GameObject[6];
    model_manager_ = TileModelManagerPrefab.GetComponent<TileModelManager>();

    if (mode == Mode.TEST_MODEL) {
      SpawnAllModels();
    }
  }

  void SpawnAllModels() {
    testing_tiles_ = new Tile[model_manager_.TileModels.Count, 1, 1];

    for (int i = 0; i < model_manager_.TileModels.Count; ++i) {
      List<TileModel> model = new List<TileModel>() {
        model_manager_.TileModels[i]
      };
      testing_tiles_[i, 0, 0] = TileFactory.Instance.CreateDefaultTile(this.transform, i * 2, 0, 0, model);
      testing_tiles_[i, 0, 0].Collapse(model_manager_.TileModels[i]);
    }
  }

  // Update is called once per frame
  void Update() {
    selected_ = Selection.activeGameObject;

    if (selected_ == null) {
      return;
    }
    FaceOrientation gizmo_orientation;
    GameObject selected_gizmo = SelectedGizmo(out gizmo_orientation);

    if (selected_gizmo != null) {
      if (mode == Mode.TEST_WAVE)
        HighlightMatchingFaces(tile_model_.Adjacencies.Adjacencies[(int)gizmo_orientation], Generator.Wave);
      if (mode == Mode.TEST_MODEL)
        HighlightMatchingFaces(tile_model_.Adjacencies.Adjacencies[(int)gizmo_orientation], testing_tiles_);

      return;
    }


    Tile tile = selected_.GetComponent<Tile>();
    if (tile == null) {
      // Allow tile models to be selected
      if (selected_.transform.parent != null) {
        tile = selected_.transform.parent.GetComponentInParent<Tile>();
        if (tile == null) {
          return;
        }
      }else {
        return;
      }
    }

    TileModel new_model = tile.Model;

    // If there is something selected
    if (new_model != null) {
      if (!new_model.Equals(tile_model_)) {
        DeleteGizmos();
        tile_model_ = new_model;
        DrawFaceGizmos();
      }
    }
    // Raycast when button down is clicked, check if some selectable object is clicked
    // If a selectable object is clicked, Draw edges and all matches in Wave.
  }

  private GameObject SelectedGizmo(out FaceOrientation gizmo_orientation) {
    for (int i = 0; i < face_gizmos_.Length; ++i) {
      GameObject gizmo = face_gizmos_[i];
      // Check if a gizmo is selected
      if (gizmo != null && gizmo.Equals(selected_)) {
        gizmo_orientation = (FaceOrientation)i;
        return gizmo;
      }
    }
    gizmo_orientation = FaceOrientation.NORTH;
    return null;
  } 

  private void HighlightMatchingFaces(FaceAdjacency adjacency, Tile[,,] tiles) {
    foreach(Tile tile in tiles) {
      FaceAdjacency other = tile.Model.Adjacencies.Adjacencies[(int)adjacency.Orientation];
      FaceAdjacency other_opposite = tile.Model.Adjacencies.Adjacencies[(int)adjacency.Orientation.Opposite()];

      if (adjacency.Match(other)) {
        other.Draw(tile.transform.position, Color.red, 1f);
      }

      if (adjacency.Match(other_opposite)) {
        other_opposite.Draw(tile.transform.position, Color.red, 1f);
      }
    }
  }

  private void DrawFaceGizmos() {
    DrawFaceGizmo(FaceOrientation.NORTH);
    DrawFaceGizmo(FaceOrientation.SOUTH);
    DrawFaceGizmo(FaceOrientation.EAST);
    DrawFaceGizmo(FaceOrientation.WEST);
    DrawFaceGizmo(FaceOrientation.TOP);
    DrawFaceGizmo(FaceOrientation.BOTTOM);
    // Draw some selectable object to choose face (N, S, E, W, T, B)
  }

  private void DrawFaceGizmo(FaceOrientation orientation) {
    // Do not draw gizmo if there are no edges on that face
    if (tile_model_.Adjacencies.Adjacencies[(int)orientation].Edges.Count != 0) {
      face_gizmos_[(int)orientation] = Instantiate(FaceGizmoPrefab, selected_.transform.position + GizmoOffset(orientation), Quaternion.identity, this.transform);
      face_gizmos_[(int)orientation].name = orientation.ToString();
    }

  }

  private Vector3 GizmoOffset(FaceOrientation orientation) {
    switch (orientation) {
      case FaceOrientation.NORTH:
        return new Vector3(-0.5f, 0, 0);
      case FaceOrientation.SOUTH:
        return new Vector3(0.5f, 0, 0);
      case FaceOrientation.EAST:
        return new Vector3(0, 0, 0.5f);
      case FaceOrientation.WEST:
        return new Vector3(0, 0, -0.5f);
      case FaceOrientation.TOP:
        return new Vector3(0, 0.5f, 0);
      case FaceOrientation.BOTTOM:
        return new Vector3(0, -0.5f, 0);
    }
    return Vector3.zero;
  }

  private void DeleteGizmos() {
    foreach(GameObject gizmo in face_gizmos_) {
      if (gizmo != null) {
        Destroy(gizmo);
      }
    }
  }
}
