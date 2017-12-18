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

  public Vertex(T data) {
    this.data = data;
    this.neighbors = new LinkedList<Edge<T>>();
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
  /// List of vertices and the component id to which they belong.
  /// </summary>
  private List<Tuple<Vertex<T>, int>> vertices_;

  /// <summary>
  /// Number of components in the graph structure
  /// </summary>
  private int n_components_;
  #endregion

  #region Getters/Setters
  public List<Tuple<Vertex<T>, int>> Vertices {
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
    vertices_ = new List<Tuple<Vertex<T>,int>>();
    n_components_ = 0;
  }

  public void Reset() {
    vertices_.Clear();
  }

  public Vertex<T> GetVertex(T data) {
    foreach(Tuple<Vertex<T>, int> tuple in vertices_) {
      if (tuple.First.data.Equals(data)) {
        return tuple.First;
      }
    }
    return null;
  }

  public Vertex<T> AddVertex(T data) {
    if (GetVertex(data) == null) {
      Vertex<T> vertex = new Vertex<T>(data);
      Tuple<Vertex<T>, int> tuple = new Tuple<Vertex<T>, int>(vertex, 0);
      vertices_.Add(tuple);
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

    if (bidirectional_edge) {
      AddEdge(v2, v1, false);
    }

    Debug.LogWarning("Edge " + v1 + " -> " + v2 + " already exists");
  }

  #endregion


  #region Private Methods

  private void UpdateConnectedComponents(Edge<T> edge) {
    // TODO Check which components does edge connects
    // If different components are connected, assign lowest component number to all vertices with those component id
  }

  #endregion
}
