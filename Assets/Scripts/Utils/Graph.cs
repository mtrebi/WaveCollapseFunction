using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge<T> {
  public Vertex<T> node1;
  public Vertex<T> node2;

  public Edge(Vertex<T> v1, Vertex<T> v2) {
    this.node1 = v1;
    this.node2 = v2;
  }
}

public class Vertex<T> {
  public T data;
  public LinkedList<Edge<T>> neighbors;

  public Vertex(T data) {
    this.data = data;
    this.neighbors = new LinkedList<Edge<T>>();
  }

  public bool AddEdge(Vertex<T> v2) {
    Edge<T> edge = new Edge<T>(this, v2);
    if (!neighbors.Contains(edge)) {
      neighbors.AddLast(edge);
      return true;
    }
    return false;
  }
}

public class Graph<T> {
  #region Public Fields
  #endregion

  #region Private Fields
  private List<Vertex<T>> vertices_;
  #endregion

  #region Getters/Setters
  public List<Vertex<T>> Vertices {
    get {
      return vertices_;
    }
  }
  #endregion

  #region Public Methods

  public Graph(){
    vertices_ = new List<Vertex<T>>();
  }

  public void Reset() {
    vertices_.Clear();
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
      Vertex<T> vertex = new Vertex<T>(data);
      vertices_.Add(vertex);
      return vertex;
    }
    Debug.LogWarning("Vertex " + data.ToString() + " already exists");
    return null;
  }

  public bool AddEdge(Vertex<T> v1, Vertex<T> v2, bool bidirectional = true) {
    if (v1.AddEdge(v2)) {
      if (bidirectional) {
        this.AddEdge(v2, v1, false);
      }
      return true;
    }

    Debug.LogWarning("Edge " + v1 + " -> " + v2 + " already exists");
    return false;
  }

  public int NumberOfGraphs() {
    // TODO Count how many closed graphs there are
    return 1;
  }

  #endregion


  #region Private Methods
  #endregion
}
