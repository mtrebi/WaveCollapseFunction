using UnityEngine;


public class CameraController : MonoBehaviour {
  #region Public Fields
  public const float speed = 2.0f;
  public const int min_zoom = 4;
  public const int max_zoom = 7;
  #endregion

  #region Private Fields
  public GameObject main_camera;
  public GameObject debug_camera;

  private Vector3 rotation_point_ = Vector3.zero;
  private float zoom_ = 0;
  private bool left_click_down_ = false;
  #endregion

  #region Getters/Setters

  public Vector3 RotationPoint {
    set {
      rotation_point_ = value;
    }
  }

  #endregion

  #region Unity Methods
  private void Awake() {
    main_camera = this.gameObject;
    debug_camera = this.transform.FindChild("Debug Camera").gameObject;
    // rotation_point is assigned from the outside everytime a new building is generated
  }

  private void Start() {
      
  }

  private void Update(){
    CameraRotation();
    CameraZoom();
  }
  #endregion

  #region Private Methods
  private void CameraZoom() {
    zoom_ -= Input.GetAxis("Mouse ScrollWheel");
    zoom_ = Mathf.Clamp(zoom_, min_zoom, max_zoom);
    main_camera.GetComponent<Camera>().orthographicSize = zoom_;
    debug_camera.GetComponent<Camera>().orthographicSize = zoom_;
  }

  
  private void CameraRotation() {
    if (Input.GetMouseButtonDown(0)) {
      left_click_down_ = true;
    }
    if (Input.GetMouseButtonUp(0)) {
      left_click_down_ = false;
    }
    if (left_click_down_) {
      main_camera.transform.RotateAround(rotation_point_, Vector3.up, Input.GetAxis("Mouse X") * speed);
    }
  }
  
  #endregion

}
 