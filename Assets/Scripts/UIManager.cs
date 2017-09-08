using UnityEngine;

public class UIManager : MonoBehaviour {
  // TODO Interactive UI to build tiles by clicking!
  // Show available states
  // Collapse to one state

  public GameObject generator;
  public GameObject camera;

  private WCFGenerator WCFGenerator_;

  private int width_ = 5, 
              height_ = 5, 
              depth_ = 5;
	// Use this for initialization
	void Start () {
    WCFGenerator_ = generator.GetComponent<WCFGenerator>();
  }

  // Update is called once per frame
  void Update() {

  }

  public void SetWidth(string width) {
    if (!System.String.IsNullOrEmpty(width)) {
      width_ = System.Int32.Parse(width);
    }
    else {
      width_ = 0;
    }
  }

  public void SetDepth(string depth) {
    if (!System.String.IsNullOrEmpty(depth)) {
      depth_ = System.Int32.Parse(depth);
    }
    else {
      depth_ = 0;
    }
  }

  public void SetHeight(string height) {
    if (!System.String.IsNullOrEmpty(height)) {
      height_ = System.Int32.Parse(height);
    }
    else {
      height_ = 0;
    }
  }

  public void ChangeZoom(float zoom) {
    camera.GetComponent<Camera>().orthographicSize = zoom;
  }

  public void Generate() {
    if (width_ != 0 &&
      depth_ != 0 &&
      height_ != 0) {

      WCFGenerator_.DestroyWave();
      WCFGenerator_.Width = width_;
      WCFGenerator_.Depth = depth_;
      WCFGenerator_.Height = height_;
      WCFGenerator_.ProgramState = ProgramState.INIT;
    }else {
      Debug.Log("Dimensions must be bigger than 0");
    }
  }
}