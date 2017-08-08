using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SymmetryType {
  X,
  T,
  I,
  L
}

/// <summary>
/// Face orientation of the imaginary cube placed in the world with X pointing out the scren, Y up and Z to the right
/// </summary>
public enum FaceOrientation {
  NORTH,
  WEST,
  SOUTH,
  EAST,
  TOP,
  BOTTOM
}

/// <summary>
/// Represents a line between two points in a 3D Space. Direction doesn-t matter. Edge(v1, v2) is equals to Edge(v2, v1)
/// </summary>
public class Edge {
  private Vector3 v1_,
                 v2_;
  // TODO face index!
  public int[] faceIndex = new int[2];

  public Vector3 v1 {
    get {
      return v1_;
    }
  }

  public Vector3 v2 {
    get {
      return v2_;
    }
  }

  public Edge() {
    v1_ = Vector3.zero;
    v2_ = Vector3.zero;
  }

  public Edge(Vector3 v1, Vector3 v2, int face_index0, int face_index1) {
    v1_ = v1;
    v2_ = v2;

    this.faceIndex[0] = face_index0;
    this.faceIndex[1] = face_index1;
  }

  public Edge (Edge edge) {
    v1_ = edge.v1_;
    v2_ = edge.v2_;

    this.faceIndex[0] = edge.faceIndex[0];
    this.faceIndex[1] = edge.faceIndex[1];
  }

  /// <summary>
  /// Sorts the vertices of the edge given the priority (from highest to lowest) x, z, y
  /// </summary>
  public void SortEdgeVertices() {
    if (CompareTo(v1_, v2_) == 1) {
      Vector3 aux = v1_;
      v1_ = v2_;
      v2_ = aux;
    }
  }

  /// <summary>
  /// Compare this edge to another edge to see which one is smaller given the priority (from highest to lowest) x, z, y
  /// </summary>
  /// <param name="edge"></param>
  /// <returns> -1 if this edge preceeds. 0 if equals. 1 if this edge is after</returns>
  public int CompareTo(Edge edge) {
    switch (CompareTo(this.v1_, edge.v1_)) {
      case 0:
        return CompareTo(this.v2_, edge.v2_);
      case -1:
        return -1;
      case 1:
        return 1;
    }
    // Should never get here
    return 0;
  }

  /// <summary>
  /// Compare a vertex with another to see which one is smaller given the priority (from highest to lowest) x, z, y
  /// </summary>
  /// <param name="v1"></param>
  /// <param name="v2"></param>
  /// <returns> -1 if this vertex preceeds. 0 if equals. 1 if this vertex is after</returns>
  private int CompareTo(Vector3 v1, Vector3 v2) {
    int result = 0;

    if (v1.x < v2.x) {
      result = -1;
    }
    else if (v1.x > v2.x) {
      result = 1;
    }
    else { // Same x
      if (v1.z < v2.z) {
        result = -1;
      }
      else if (v1.z > v2.z) {
        result = 1;
      }
      else { // Same x and z
        if (v1.y < v2.y) {
          result = -1;
        }
        else if (v1.y > v2.y) {
          result = 1;
        }
      }
    }
    return result;
  }

  public override string ToString() {
    return v1.ToString() + " <--> " + v2.ToString();
  }

  public override bool Equals(object obj) {
    var item = obj as Edge;
    if (item == null) return false;

    return (this.v1_ == item.v1_ && this.v2_ == item.v2_)
           || (this.v1_ == item.v2_ && this.v2 == item.v1_);
  }

  public override int GetHashCode() {
    int hash1, hash2;
    unchecked // Overflow is fine, just wrap
    {
      hash1 = (int)2166136261;
      // Suitable nullity checks etc, of course :)
      // Suitable nullity checks etc, of course :)
      hash1 = (hash1 * 16777619) + System.Math.Round(v1.x, 5).GetHashCode();
      hash1 = (hash1 * 16777619) + System.Math.Round(v1.y, 5).GetHashCode();
      hash1 = (hash1 * 16777619) + System.Math.Round(v1.z, 5).GetHashCode();

      hash2 = (int)2166136261;
      // Suitable nullity checks etc, of course :)
      // Suitable nullity checks etc, of course :)
      hash2 = (hash2 * 16777619) + System.Math.Round(v2.x, 5).GetHashCode();
      hash2 = (hash2 * 16777619) + System.Math.Round(v2.y, 5).GetHashCode();
      hash2 = (hash2 * 16777619) + System.Math.Round(v2.z, 5).GetHashCode();
    }

    return hash1 + hash2;
  }
}

[System.Serializable]
public class FaceAdjacency {
  [SerializeField] private int edges_id_ = -1;
  /// <summary>
  /// Index of the used dimensions (faces are 2D so two dimension are needed)
  /// </summary>
  private int dimension1_ = 0,
              dimension2_ = 0,
              unused_dimension_ = 0;
  [SerializeField] private FaceOrientation face_orientation_;
  /// <summary>
  /// Stores the original edges from the mesh that make up the face
  /// </summary>
  private List<Edge> edges;

  public int EdgesId {
    get {
      return edges_id_;
    }
  }

  public FaceOrientation Orientation {
    get {
      return face_orientation_;
    }
  }

  public List<Edge> Edges {
    get {
      return edges;
    }
  }

  public FaceAdjacency(FaceOrientation face_orientation) {
    face_orientation_ = face_orientation;
    edges = new List<Edge>();
    CalculateFaceDimensions();
  }

  public void AddEdge(Edge edge) {
    edge.SortEdgeVertices();
    edges.Add(edge);
  }

  /// <summary>
  /// Post process the face data to speed up comparisions
  /// </summary>
  public void PostProcess() {
    // Sort edges
    edges.Sort(delegate (Edge edge1, Edge edge2) {
      return edge1.CompareTo(edge2);
    });

    // Calculate Face ID
    // First and second dimension should be equals
    // Third dimension should lay on one side (-.5 o .5) and be equals
    edges_id_ = 0;
    for (int i = 0; i < edges.Count; ++i) {
      Vector3 v1 = new Vector3(edges[i].v1[dimension1_], edges[i].v1[dimension2_], Mathf.Abs(edges[i].v1[unused_dimension_]));
      Vector3 v2 = new Vector3(edges[i].v2[dimension1_], edges[i].v2[dimension2_], Mathf.Abs(edges[i].v1[unused_dimension_]));

      if (v1.z == .5 && v2.z == .5) {
        // Edge lays on a side
        Edge new_edge = new Edge(v1, v2, edges[i].faceIndex[0], edges[i].faceIndex[1]);
        edges_id_ = edges_id_.GetHashCode() + new_edge.GetHashCode();
      }
    }
  }

  public bool Match(FaceAdjacency other_face) {
    // TODO add equals method to make sure it avoids hash collisions
    return this.edges_id_ == other_face.edges_id_; // Compare original edges and dont care dimension to be 0.5 or -0.5
  }

  public void Draw(Color color) {
    foreach (Edge edge in edges) {
      Debug.DrawLine(edge.v1, edge.v2, color, 1.0f, false);
    }
  }

  public override bool Equals(object obj) {
    var item = obj as FaceAdjacency;
    if (item == null) return false;

    if (!face_orientation_.Equals(item.face_orientation_)) {
      return false;
    }

    if (edges.Count != item.edges.Count) {
      return false;
    }

    for (int i = 0; i < edges.Count; ++i) {
      if (edges[i].Equals(item.edges[i])) {
        return false;
      }
    }

    return true;
  }

  public override int GetHashCode() {
    // TODO Review original edges get hash code
    return face_orientation_.GetHashCode() + edges.GetHashCode();
  }

  public override string ToString() {
    return face_orientation_.ToString() + " " + edges_id_;
  }

  private void CalculateFaceDimensions() {
    switch (face_orientation_) {
      case FaceOrientation.TOP:
      case FaceOrientation.BOTTOM:
        // y is not used
        dimension1_ = 0;
        unused_dimension_ = 1;
        dimension2_ = 2;
        break;
      case FaceOrientation.NORTH:
      case FaceOrientation.SOUTH:
        // x is not used
        unused_dimension_ = 0;
        dimension1_ = 1;
        dimension2_ = 2;
        break;
      case FaceOrientation.EAST:
      case FaceOrientation.WEST:
        // z is not used
        dimension1_ = 0;
        dimension2_ = 1;
        unused_dimension_ = 2;
        break;
    }
  }
}

[System.Serializable]
public class TileAdjacencies {
  [SerializeField] private FaceAdjacency[] adjacencies_;

  public FaceAdjacency[] Adjacencies {
    get {
      return adjacencies_;
    }
  }

  private void InitializeAdjacencies() {
    adjacencies_ = new FaceAdjacency[6];
    adjacencies_[(int)FaceOrientation.NORTH] = new FaceAdjacency(FaceOrientation.NORTH);
    adjacencies_[(int)FaceOrientation.SOUTH] = new FaceAdjacency(FaceOrientation.SOUTH);
    adjacencies_[(int)FaceOrientation.EAST] = new FaceAdjacency(FaceOrientation.EAST);
    adjacencies_[(int)FaceOrientation.WEST] = new FaceAdjacency(FaceOrientation.WEST);
    adjacencies_[(int)FaceOrientation.TOP] = new FaceAdjacency(FaceOrientation.TOP);
    adjacencies_[(int)FaceOrientation.BOTTOM] = new FaceAdjacency(FaceOrientation.BOTTOM);
  }

  public TileAdjacencies(Mesh mesh) {
    InitializeAdjacencies();
    CalculateAdjacencies(mesh);
  }

  public TileAdjacencies(TileAdjacencies tile_adjacencies, int y_rotation) {
    InitializeAdjacencies();

    for (int adjacency_i = 0; adjacency_i < tile_adjacencies.Adjacencies.Length; ++adjacency_i) {
      FaceAdjacency adjacency = tile_adjacencies.Adjacencies[adjacency_i];
      for (int edge_i = 0; edge_i < adjacency.Edges.Count; ++edge_i) {
        Edge rotated_edge;
        FaceOrientation rotated_orientation;
        //TODO delete Edge test_edge = new Edge(new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, -0.5f, 0.5f), 0, 0);
        RotateEdge(out rotated_edge, out rotated_orientation, adjacency.Edges[edge_i], adjacency.Orientation, y_rotation);
        adjacencies_[(int) rotated_orientation].Edges.Add(rotated_edge);
      }
    }

    for (int i = 0; i < adjacencies_.Length; ++i) {
      adjacencies_[i].PostProcess();
    }
  }

  private void RotateEdge(out Edge rotated_edge, out FaceOrientation rotated_orientation, Edge original_edge, FaceOrientation original_orientation, int y_rotation) {
    int rotation_steps = Mathf.Abs(y_rotation / 90);
    rotated_orientation = (FaceOrientation)(((int)original_orientation + rotation_steps) % 4);
    Vector3 v1 = Mathematics.RotateAroundPoint(original_edge.v1, Vector3.up, new Vector3(0, y_rotation, 0));
    Vector3 v2 = Mathematics.RotateAroundPoint(original_edge.v2, Vector3.up, new Vector3(0, y_rotation, 0));

    rotated_edge = new Edge(v1, v2, original_edge.faceIndex[0], original_edge.faceIndex[1]);
  }

  private void CalculateAdjacencies(Mesh mesh) {
    Edge[] outer_edges = BuildManifoldEdges(mesh);

    foreach (Edge edge in outer_edges) {
      FaceOrientation[] involved_faces = GetFaceOrientationOfEdge(edge);
      foreach (FaceOrientation face_orientation in involved_faces) {
        adjacencies_[(int)face_orientation].AddEdge(edge);
      }
    }

    for (int i = 0; i < adjacencies_.Length; ++i) {
      adjacencies_[i].PostProcess();
    }
  }

  /// <summary>
  /// Checks on which side of the imaginary cube the edge lays
  /// </summary>
  /// <param name="edge"> Edge to check </param>
  /// <returns> Returns on which side the edge lays. Can return None if the edge doesnt lay on any side </returns>
  private FaceOrientation[] GetFaceOrientationOfEdge(Edge edge) {
    List<FaceOrientation> faces = new List<FaceOrientation>();

    if (edge.v1.x == edge.v2.x && edge.v1.x == 0.5) {
      faces.Add(FaceOrientation.SOUTH);
    }
    else if (edge.v1.x == edge.v2.x && edge.v1.x == -0.5) {
      faces.Add(FaceOrientation.NORTH);
    }

    if (edge.v1.z == edge.v2.z && edge.v1.z == 0.5) {
      faces.Add(FaceOrientation.WEST);
    }
    else if (edge.v1.z == edge.v2.z && edge.v1.z == -0.5) {
      faces.Add(FaceOrientation.EAST);
    }

    if (edge.v1.y == edge.v2.y && edge.v1.y == 0.5) {
      faces.Add(FaceOrientation.TOP);
    }
    else if (edge.v1.y == edge.v2.y && edge.v1.y == -0.5) {
      faces.Add(FaceOrientation.BOTTOM);
    }

    return faces.ToArray();
  }

  /// Builds an array of edges that connect to only one triangle.
  /// In other words, the outline of the mesh    
  private static Edge[] BuildManifoldEdges(Mesh mesh) {
    // Build a edge list for all unique edges in the mesh
    Edge[] all_edges = BuildEdges(mesh);

    HashSet<Edge> culledEdges = new HashSet<Edge>();

    foreach (Edge edge in all_edges) {
      if (edge.faceIndex[0] == edge.faceIndex[1]) {
        culledEdges.Add(edge);
      }
    }

    Edge[] edges = new Edge[culledEdges.Count];
    culledEdges.CopyTo(edges);

    /* TODO remove test code
    int[] hashes = new int[edges.Length];
    for (int i = 0; i < hashes.Length; ++i) {
      hashes[i] = edges[i].GetHashCode();
    }
    */

    return edges;
  }

  /// Builds an array of unique edges
  /// This requires that your mesh has all vertices welded. However on import, Unity has to split
  /// vertices at uv seams and normal seams. Thus for a mesh with seams in your mesh you
  /// will get two edges adjoining one triangle.
  /// Often this is not a problem but you can fix it by welding vertices 
  /// and passing in the triangle array of the welded vertices.
  private static Edge[] BuildEdges(Mesh mesh) {
    int vertexCount = mesh.vertexCount;
    int[] triangleArray = mesh.triangles;
    int maxEdgeCount = triangleArray.Length;
    int[] firstEdge = new int[vertexCount + maxEdgeCount];
    int nextEdge = vertexCount;
    int triangleCount = triangleArray.Length / 3;

    for (int a = 0; a < vertexCount; a++)
      firstEdge[a] = -1;

    // First pass over all triangles. This finds all the edges satisfying the
    // condition that the first vertex index is less than the second vertex index
    // when the direction from the first vertex to the second vertex represents
    // a counterclockwise winding around the triangle to which the edge belongs.
    // For each edge found, the edge index is stored in a linked list of edges
    // belonging to the lower-numbered vertex index i. This allows us to quickly
    // find an edge in the second pass whose higher-numbered vertex index is i.
    Edge[] edgeArray = new Edge[maxEdgeCount];

    int edgeCount = 0;
    for (int a = 0; a < triangleCount; a++) {
      int i1 = triangleArray[a * 3 + 2];
      for (int b = 0; b < 3; b++) {
        int i2 = triangleArray[a * 3 + b];
        if (i1 < i2) {
          // asdfafafafa
          Edge newEdge = new Edge(mesh.vertices[i1], mesh.vertices[i2], a, a);
          edgeArray[edgeCount] = newEdge;

          int edgeIndex = firstEdge[i1];
          if (edgeIndex == -1) {
            firstEdge[i1] = edgeCount;
          }
          else {
            while (true) {
              int index = firstEdge[nextEdge + edgeIndex];
              if (index == -1) {
                firstEdge[nextEdge + edgeIndex] = edgeCount;
                break;
              }

              edgeIndex = index;
            }
          }

          firstEdge[nextEdge + edgeCount] = -1;
          edgeCount++;
        }

        i1 = i2;
      }
    }

    // Second pass over all triangles. This finds all the edges satisfying the
    // condition that the first vertex index is greater than the second vertex index
    // when the direction from the first vertex to the second vertex represents
    // a counterclockwise winding around the triangle to which the edge belongs.
    // For each of these edges, the same edge should have already been found in
    // the first pass for a different triangle. Of course we might have edges with only one triangle
    // in that case we just add the edge here
    // So we search the list of edges
    // for the higher-numbered vertex index for the matching edge and fill in the
    // second triangle index. The maximum number of comparisons in this search for
    // any vertex is the number of edges having that vertex as an endpoint.

    for (int a = 0; a < triangleCount; a++) {
      int i1 = triangleArray[a * 3 + 2];
      for (int b = 0; b < 3; b++) {
        int i2 = triangleArray[a * 3 + b];
        if (i1 > i2) {
          bool foundEdge = false;
          for (int edgeIndex = firstEdge[i2]; edgeIndex != -1; edgeIndex = firstEdge[nextEdge + edgeIndex]) {
            Edge edge = edgeArray[edgeIndex];
            if ((edge.v2 == mesh.vertices[i1]) && (edge.faceIndex[0] == edge.faceIndex[1])) {
              edgeArray[edgeIndex].faceIndex[1] = a;
              foundEdge = true;
              break;
            }
          }

          if (!foundEdge) {
            Edge newEdge = new Edge(mesh.vertices[i1], mesh.vertices[i2], a, a);
            edgeArray[edgeCount] = newEdge;
            edgeCount++;
          }
        }
        i1 = i2;
      }
    }

    Edge[] compactedEdges = new Edge[edgeCount];
    for (int e = 0; e < edgeCount; e++) {
      compactedEdges[e] = edgeArray[e];
    }

    return compactedEdges;
  }
}

[System.Serializable]
public class TileModel {
  private GameObject prefab_;
  private Quaternion prefab_orientation_;
  private float probability_;
  private TileAdjacencies adjacencies_;

  public TileAdjacencies Adjacencies {
    get {
      return adjacencies_;
    }
  }

  public TileModel(GameObject prefab, float probability) {
    prefab_ = prefab;
    probability_ = probability;
    adjacencies_ = new TileAdjacencies(prefab_.GetComponent<MeshFilter>().sharedMesh);
  }

  public TileModel(GameObject prefab, Quaternion orientation, float probability, TileAdjacencies adjacencies) {
    prefab_ = prefab;
    prefab_orientation_ = orientation;
    probability_ = probability;
    adjacencies_ = adjacencies;
  }

  public TileModel[] GetSymmetricModels(SymmetryType symmetry) {
    TileModel[] models = new TileModel[GetCardinality(symmetry)];

    models[0] = new TileModel(prefab_, Quaternion.identity, probability_, adjacencies_);

    for (int i = 1; i < models.Length; ++i) {
      int y_rotation = 90 * i;
      models[i] = GenerateModel(y_rotation);
    }

    return models;
  }

  private int GetCardinality(SymmetryType symmetry) {
    int cardinality = 0;
    switch (symmetry) {
      case SymmetryType.X:
        cardinality = 1;
        break;
      case SymmetryType.L:
        cardinality = 4;
        break;
      case SymmetryType.T:
        cardinality = 4;
        break;
      case SymmetryType.I:
        cardinality = 2;
        break;
    }
    return cardinality;
  }

  private TileModel GenerateModel(int y_rotation) {
    TileAdjacencies instance_adjacencies = new TileAdjacencies(adjacencies_, y_rotation);
    TileModel model = new TileModel(prefab_, Quaternion.Euler(0, y_rotation, 0), probability_, instance_adjacencies);
    return model;
  }
}
