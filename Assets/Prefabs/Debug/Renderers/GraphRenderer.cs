using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphRenderer : MonoBehaviour {
  #region Public Fields
  public WCFGenerator generator;
  public GameObject VertexPrefab;
  public GameObject EdgePrefab;
  #endregion

  #region Private Fields
  bool render_ = false;
  Graph<Tile> graph_;
  Dictionary<Vector3, GameObject> rendered_objects_;

  private Color32[] colors_ = new Color32[]{
    new Color32(230, 25, 75, 255),
    new Color32(60, 180, 75, 255),
    new Color32(255, 225, 25, 255),
    new Color32(0, 130, 200, 255),
    new Color32(245, 130, 48, 255),
    new Color32(145, 30, 180, 255),
    new Color32(70, 240, 240, 255),
    new Color32(240, 50, 230, 255),
    new Color32(210, 245, 60, 255),
    new Color32(250, 190, 190, 255),
    new Color32(0, 128, 128, 255),
    new Color32(230, 190, 255, 255),
    new Color32(170, 110, 40, 255),
    new Color32(255, 250, 200, 255),
    new Color32(128, 0, 0, 255),
    new Color32(170, 255, 195, 255),
    new Color32(128, 128, 0, 255),
    new Color32(255, 215, 180, 255),
    new Color32(0, 0, 128, 255),
    new Color32(128, 128, 128, 255),
    new Color32(255, 255, 255, 255),
    new Color32(0, 0, 0, 255),
  };

  #endregion

  #region Getters/Setters

  public void SetRender(bool render) {
    render_ = render;
    UpdateRenderedObjectStatus(render_);
  }

  #endregion

  #region Unity Methods
  private void Awake() {
    rendered_objects_ = new Dictionary<Vector3, GameObject>();
  }

  void Start () {
    graph_ = generator.Graph;
	}
	
	void Update () {
    if (!generator.ProgramState.Equals(ProgramState.RUNNING)){
      return;
    }
    Reset();
    Render();
  }
  #endregion

  #region Private Methods

  private void UpdateRenderedObjectStatus(bool active) {
    foreach(GameObject obj in rendered_objects_.Values) {
      obj.SetActive(active);
    }
  }

  private void Render() {
    foreach(Vertex<Tile> vertex in graph_.Vertices) {
      RenderVertex(vertex);
      foreach(Edge<Tile> edge in vertex.neighbors) {
        if (!AlreadyRendered(edge)) {
          RenderEdge(edge);
        }
      }
    }
  }

  private void Reset() {
    foreach(GameObject obj in rendered_objects_.Values) {
      Destroy(obj);
    }
    rendered_objects_.Clear();
  }

  private bool AlreadyRendered(Vertex<Tile> vertex) {
    return rendered_objects_.ContainsKey(GetPosition(vertex));
  }

  private bool AlreadyRendered(Edge<Tile> edge) {
    return rendered_objects_.ContainsKey(GetPosition(edge));
  }

  private void RenderVertex(Vertex<Tile> vertex) {
    Vector3 position = GetPosition(vertex);
    GameObject obj = Instantiate(VertexPrefab, position, Quaternion.identity, this.transform);
    obj.SetActive(render_);
    obj.layer = LayerMask.NameToLayer("Debug");
    ChangeObjColor(obj, CalculateColorComponent(vertex.component_id));
    rendered_objects_.Add(position, obj);
  }

  private void DeleteVertex(Vertex<Tile> vertex) {
    Vector3 position = GetPosition(vertex);
    rendered_objects_.Remove(position);
  }

  private void RenderEdge(Edge<Tile> edge) {
    Vector3 position = GetPosition(edge);
    GameObject obj = Instantiate(EdgePrefab, position, EdgeRotation(edge), this.transform);
    obj.SetActive(render_);
    obj.layer = LayerMask.NameToLayer("Debug");
    ChangeObjColor(obj, CalculateColorComponent(edge.node1.component_id));
    rendered_objects_.Add(position, obj);
  }

  private void DeleteEdge(Edge<Tile> edge) {
    // TODO Delete edge
    throw new System.NotImplementedException();
  }

  private Vector3 GetPosition(Vertex<Tile> vertex) {
    Vector3 position = new Vector3(vertex.data.X, vertex.data.Y, vertex.data.Z);
    return position;
  }

  private Vector3 GetPosition(Edge<Tile> edge) {
    Vector3 position1 = new Vector3(edge.node1.data.X, edge.node1.data.Y, edge.node1.data.Z);
    Vector3 position2 = new Vector3(edge.node2.data.X, edge.node2.data.Y, edge.node2.data.Z);

    Vector3 pos = (position1 + position2) / 2;

    return pos;
  }

  private Quaternion EdgeRotation(Edge<Tile> edge) {
    if (edge.node1.data.X == edge.node2.data.X &&
        edge.node1.data.Y == edge.node2.data.Y &&
        edge.node1.data.Z != edge.node2.data.Z) {
      return Quaternion.Euler(90, 0, 0); ;
    }

    if (edge.node1.data.X == edge.node2.data.X &&
        edge.node1.data.Y != edge.node2.data.Y &&
        edge.node1.data.Z == edge.node2.data.Z) {
      return Quaternion.Euler(0, 0, 0); ;
    }

    if (edge.node1.data.X != edge.node2.data.X &&
        edge.node1.data.Y == edge.node2.data.Y &&
        edge.node1.data.Z == edge.node2.data.Z) {
      return Quaternion.Euler(0, 0, 90); ;
    }

    return Quaternion.identity;
  }

  /// <summary>
  /// Calculate Graph Component color based on component id
  /// </summary>
  /// <param name="component_id"></param>
  /// <returns></returns>
  private Color32 CalculateColorComponent(int component_id) {
    return colors_[component_id % colors_.Length];
  }

  private void ChangeObjColor(GameObject obj, Color32 color) {
    Renderer rend = obj.GetComponent<Renderer>();
    rend.material.shader = Shader.Find("Unlit/Color");
    rend.material.color = color;
  }

  #endregion
}
