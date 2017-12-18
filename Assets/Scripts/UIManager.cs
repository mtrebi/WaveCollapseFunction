using UnityEngine;

public class UIManager : MonoBehaviour {
  // TODO Interactive UI to build tiles by clicking!
  // Show available states
  // Collapse to one state
  #region Public Fields
  public GameObject generator;
  public GameObject main_camera;
  public GameObject debug_camera;

  public const int min_zoom = 4;
  public const int max_zoom = 7;

  #endregion

  #region Private Fields
  private WCFGenerator WCFGenerator_;

  private int width_ = 8, 
              height_ = 5, 
              depth_ = 8;

  public float zoom_ = 0;
  #endregion

  #region Unity Methods
  void Start () {
    WCFGenerator_ = generator.GetComponent<WCFGenerator>();
  }

  void Update() {
    UpdateZoomFromMouseWheel();
  }

  #endregion

  #region Private Methods

  private void UpdateZoomFromMouseWheel() {
    zoom_ -= Input.GetAxis("Mouse ScrollWheel");
    zoom_ = Mathf.Clamp(zoom_, min_zoom, max_zoom);
    main_camera.GetComponent<Camera>().orthographicSize = zoom_;
    debug_camera.GetComponent<Camera>().orthographicSize = zoom_;
  }

  private void SetWidth(string width) {
    if (!System.String.IsNullOrEmpty(width)) {
      width_ = System.Int32.Parse(width);
    }
    else {
      width_ = 0;
    }
  }

  private void SetDepth(string depth) {
    if (!System.String.IsNullOrEmpty(depth)) {
      depth_ = System.Int32.Parse(depth);
    }
    else {
      depth_ = 0;
    }
  }

  private void SetHeight(string height) {
    if (!System.String.IsNullOrEmpty(height)) {
      height_ = System.Int32.Parse(height);
    }
    else {
      height_ = 0;
    }
  }

  private void Generate() {
    if (width_ != 0 &&
      depth_ != 0 &&
      height_ != 0) {

      WCFGenerator_.ResetWave();
      WCFGenerator_.Width = width_;
      WCFGenerator_.Depth = depth_;
      WCFGenerator_.Height = height_;
      WCFGenerator_.ProgramState = ProgramState.INIT;
    }else {
      Debug.Log("Dimensions must be bigger than 0");
    }
  }
  #endregion
}