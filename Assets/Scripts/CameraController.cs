using UnityEngine;


public class CameraController : MonoBehaviour {
  #region Public Fields
  public const int min_zoom = 4;
  public const int max_zoom = 7;
  #endregion

  #region Private Fields
  private float zoom_ = 0;
  public GameObject main_camera;
  public GameObject debug_camera;
  #endregion

  #region Getters/Setters

  #endregion

  #region Unity Methods
  private void Awake() {
    main_camera = this.gameObject;
    debug_camera = this.transform.FindChild("Debug Camera").gameObject;
  }

  private void Start() {
      
  }

  private void Update(){
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

  /*
    private void RotateCameraAround() {
    if (Input.GetMouseButtonDown(0)) {
      Vector3 rotation_point = new Vector3(width_, 0, depth_) / 2;
      camera_yaw_ += camera_rotation_speed * Input.GetAxis("Mouse X");
      main_camera.transform.RotateAround(rotation_point, Vector3.up, camera_yaw_ * Time.deltaTime);
    }
  }
  */
  #endregion

}
 