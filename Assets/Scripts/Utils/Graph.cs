using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directed edge from Node1 to Node2
/// </summary>
/// <typeparam name="T"></typeparam>
public class Edge<T> {
  public Vertex<T> node1;
  public Vertex<T> node2;

  public Edge(Vertex<T> v1, Vertex<T> v2) {
    this.node1 = v1;
    this.node2 = v2;
  }

  public override bool Equals(object obj) {
    var item = obj as Edge<T>;

    if (item == null) {
      return false;
    }

    return this.node1 == item.node1 && this.node2 == item.node2;
  }

  public override int GetHashCode() {
    return this.node1.GetHashCode() / this.node2.GetHashCode();
  }

}

public class Vertex<T> {
  public T data;
  public LinkedList<Edge<T>> neighbors;
  /// <summary>
  /// 
  /// </summary>
  public int component_id = -1;

  public Vertex(T data, int component_id) {
    this.data = data;
    this.neighbors = new LinkedList<Edge<T>>();
    this.component_id = component_id;
  }

  public Edge<T> AddEdge(Vertex<T> v2) {
    Edge<T> edge = new Edge<T>(this, v2);
    if (!neighbors.Contains(edge)) {
      neighbors.AddLast(edge);
      return edge;
    }
    return null;
  }
}

public class Graph<T> {
  #region Public Fields
  #endregion

  #region Private Fields
  /// <summary>
  /// List of vertices
  /// </summary>
  private List<Vertex<T>> vertices_;

  /// <summary>
  /// Number of components in the graph structure
  /// </summary>
  private int n_components_;
  #endregion

  #region Getters/Setters
  public List<Vertex<T>> Vertices {
    get {
      return vertices_;
    }
  }

  public int NComponents {
    get {
      return n_components_;
    }
  }

  #endregion

  #region Public Methods

  public Graph(){
    vertices_ = new List<Vertex<T>>();
    n_components_ = -1;
  }

  public void Reset() {
    vertices_.Clear();
    n_components_ = -1;
  }

  public Vertex<T> GetVertex(T data) {
    foreach(Vertex<T> vertex in vertices_) {
      if (vertex.data.Equals(data)) {
        return vertex;
      }
    }
    return null;
  }

  public Vertex<T> AddVertex(T data) {
    if (GetVertex(data) == null) {
      Vertex<T> vertex = new Vertex<T>(data, ++n_components_);
      vertices_.Add(vertex);
      return vertex;
    }
    Debug.LogWarning("Vertex " + data.ToString() + " already exists");
    return null;
  }

  public void AddEdge(Vertex<T> v1, Vertex<T> v2, bool bidirectional_edge = true) {
    Edge<T> new_edge = v1.AddEdge(v2);
    if (new_edge != null) {
      UpdateConnectedComponents(new_edge);
    }
    else {
      Debug.LogWarning("Edge " + v1 + " -> " + v2 + " already exists");
    }

    if (bidirectional_edge) {
      AddEdge(v2, v1, false);
    }
  }

  #endregion


  #region Private Methods

  private void UpdateConnectedComponents(Edge<T> edge) {
    // If connected nodes belong to different components
    if (edge.node1.component_id != edge.node2.component_id) {
      int min_component_id = Mathf.Min(edge.node1.component_id, edge.node2.component_id);
      ChangeComponentId(edge.node1, min_component_id);
      ChangeComponentId(edge.node2, min_component_id);
      --n_components_;
    }
  }

  /// <summary>
  /// Sets the component id of all nodes that belong to the same component as the given vertex
  /// </summary>
  /// <param name="origin"></param>
  /// <param name="component_id"></param>
  private void ChangeComponentId(Vertex<T> origin, int component_id) {
    Stack<Vertex<T>> remaining_nodes = new Stack<Vertex<T>>();
    HashSet<Vertex<T>> visited_nodes = new HashSet<Vertex<T>>();
    remaining_nodes.Push(origin);

    while (remaining_nodes.Count > 0) {
      Vertex<T> node = remaining_nodes.Pop();
      visited_nodes.Add(node);

      node.component_id = component_id;

      foreach(Edge<T> neighbor in node.neighbors) {
        if (!visited_nodes.Contains(neighbor.node2)) {
          remaining_nodes.Push(neighbor.node2);
        }
      }
    }
  }

  #endregion
}
