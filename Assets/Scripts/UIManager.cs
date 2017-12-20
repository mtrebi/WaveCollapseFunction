using UnityEngine;

public class UIManager : MonoBehaviour {
  // TODO Interactive UI to build tiles by clicking!
  // Show available states
  // Collapse to one state
  #region Public Fields
  public GameObject generator;
  #endregion

  #region Private Fields
  private WCFGenerator WCFGenerator_;

  private int width_  = 8, 
              height_ = 5, 
              depth_  = 8;
  #endregion

  #region Unity Methods
  void Start () {
    WCFGenerator_ = generator.GetComponent<WCFGenerator>();
  }

  void Update() {
    UpdateDebugPanelState();
  }

  #endregion

  #region Private Methods

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

      Camera.main.GetComponent<CameraController>().RotationPoint = new Vector3(width_, 0, depth_) / 2;
      WCFGenerator_.ResetWave();
      WCFGenerator_.Width = width_;
      WCFGenerator_.Depth = depth_;
      WCFGenerator_.Height = height_;
      WCFGenerator_.ProgramState = ProgramState.INIT;
    }else {
      Debug.Log("Dimensions must be bigger than 0");
    }
  }
  
  private void UpdateDebugPanelState() {
    if (Input.GetKeyUp("d")) {
      GameObject debug_panel = this.transform.FindChild("Canvas").FindChild("Debug Panel").gameObject;
      debug_panel.SetActive(!debug_panel.activeSelf);
    }
  }
  
  #endregion
}