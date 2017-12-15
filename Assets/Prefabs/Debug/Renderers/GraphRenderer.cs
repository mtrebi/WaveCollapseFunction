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
  Graph<Tile> graph_;
  Dictionary<Vector3, GameObject> rendered_objects_;
  #endregion

  #region Getters/Setters
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
    obj.layer = LayerMask.NameToLayer("Debug");
    rendered_objects_.Add(position, obj);
  }

  private void DeleteVertex(Vertex<Tile> vertex) {
    Vector3 position = GetPosition(vertex);
    rendered_objects_.Remove(position);
  }

  private void RenderEdge(Edge<Tile> edge) {
    Vector3 position = GetPosition(edge);

    GameObject obj = Instantiate(EdgePrefab, position, EdgeRotation(edge), this.transform);
    obj.layer = LayerMask.NameToLayer("Debug");
    rendered_objects_.Add(position, obj);
  }

  private void DeleteEdge(Edge<Tile> edge) {

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

  #endregion
}
