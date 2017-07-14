using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour {
  public GameObject state_prefab_;
  private GameObject state_list_;

	// Use this for initialization
	void Start () {
    state_list_ = this.transform.FindChild("Canvas/StatesList/StateListGrid").gameObject;
    AssetPreview.SetPreviewTextureCacheSize(1000);
  }

  // Update is called once per frame
  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit)) {
        Tile tile =  hit.transform.gameObject.GetComponent<Tile>();
        // TODO> Tile has collapsed!
        UpdateStatesUI(tile.AvailableStates);
      }else {
        UpdateStatesUI(null);
      }
    }
  }

  private void UpdateStatesUI(List<TileState> states) {
    ClearStatesUI();

    if (states == null) {
      return;
    }

    foreach(TileState state in states) {
      GameObject state_ui = Instantiate(state_prefab_);
      state_ui.transform.SetParent(state_list_.transform);
      state_ui.GetComponentInChildren<Text>().text = state.ToString();
    }
  }

  private void ClearStatesUI() {
    foreach (Transform child in state_list_.transform) {
      Destroy(child.gameObject);
    }
  }
}
