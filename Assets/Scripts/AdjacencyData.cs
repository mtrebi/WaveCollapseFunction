using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Face orientation of the imaginary cube placed in the world with X pointing out the scren, Y up and Z to the right
/// </summary>
public enum FaceOrientation {
  NORTH,
  WEST,
  SOUTH,
  EAST,
  TOP,
  BOTTOM,
  NONE
}

/// <summary>
/// Represents a line between two points in a mesh. Does not have direction
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

  public override bool Equals(object obj) {
    var item = obj as Edge;
    if (item == null) return false;

    return this.v1_.Equals(item.v1_) && this.v2_.Equals(item.v2_)
           || this.v1_.Equals(item.v2_) && this.v2_.Equals(item.v1_);
  }

  public override int GetHashCode() {
    return v1_.GetHashCode() + v2_.GetHashCode();
  }
}

public class FaceAdjacency {
  private int edges_id = 0;
  /// <summary>
  /// Index of the unused dimension (faces are 2D so one dimension is not used)
  /// </summary>
  private int unused_dimension_ = 0;
  private FaceOrientation face_orientation_;
  /// <summary>
  /// Stores the original edges from the mesh that make up the face
  /// </summary>
  private List<Edge> original_edges_;
  /// <summary>
  /// Stores a slightly modified version of the edges to speedup face comparision. The modification consists in replacing by 0 the unused dimension.
  /// </summary>
  private List<Edge> match_edges_;

  public int EdgesId {
    get {
      return edges_id;
    }
  }

  public FaceAdjacency(FaceOrientation face_orientation) {
    face_orientation_ = face_orientation;
    original_edges_ = new List<Edge>();
    CalculateUnusedDimension();
  }

  public void AddEdge(Edge edge) {
    edge.SortEdgeVertices();
    original_edges_.Add(edge);
  }

  /// <summary>
  /// Post process the face data to speed up comparisions
  /// </summary>
  public void PostProcess() {
    // Sort edges
    original_edges_.Sort(delegate (Edge edge1, Edge edge2) {
      return edge1.CompareTo(edge2);
    });

    match_edges_ = new List<Edge>(original_edges_.Count);

    // Set to zero unused dimension to ease comparision
    for (int i = 0; i < original_edges_.Count; ++i) {
      Vector3 v1 = original_edges_[i].v1;
      Vector3 v2 = original_edges_[i].v2;
      v1[unused_dimension_] = 0;
      v2[unused_dimension_] = 0;

      Edge new_edge = new Edge(v1, v2, original_edges_[i].faceIndex[0], original_edges_[i].faceIndex[1]);
      match_edges_.Insert(i, new_edge);
    }

    CalculateEdgesId();
  }

  public void Draw(Color color) {
    foreach (Edge edge in original_edges_) {
      Debug.DrawLine(edge.v1, edge.v2, color, 1000.0f, false);
    }
  }

  public override bool Equals(object obj) {
    var item = obj as FaceAdjacency;
    if (item == null) return false;

    if (!face_orientation_.Equals(item.face_orientation_)) {
      return false;
    }

    if (original_edges_.Count != item.original_edges_.Count) {
      return false;
    }

    for (int i = 0; i < original_edges_.Count; ++i) {
      if (original_edges_[i].Equals(item.original_edges_[i])) {
        return false;
      }
    }

    return true;
  }

  public override int GetHashCode() {
    return face_orientation_.GetHashCode() + original_edges_.GetHashCode();
  }

  private void CalculateEdgesId() {
    for (int i = 0; i < match_edges_.Count; ++i) {
      edges_id += match_edges_[i].GetHashCode();
    }
  }

  private void CalculateUnusedDimension() {
    switch (face_orientation_) {
      case FaceOrientation.TOP:
      case FaceOrientation.BOTTOM:
        unused_dimension_ = 1; // y
        break;
      case FaceOrientation.NORTH:
      case FaceOrientation.SOUTH:
        unused_dimension_ = 0; // x
        break;
      case FaceOrientation.EAST:
      case FaceOrientation.WEST:
        unused_dimension_ = 2; // z
        break;
    }
  }
}

[RequireComponent(typeof(MeshFilter))]
public class AdjacencyData : MonoBehaviour {
  private FaceAdjacency[] adjacencies_;

  // Use this for initialization
  void Start () {
    Initialize();
    CalculateAdjacencies();
  }

  // Update is called once per frame
  void Update () {
		
	}

  public void Initialize() {
    adjacencies_ = new FaceAdjacency[6];
    adjacencies_[(int)FaceOrientation.NORTH] = new FaceAdjacency(FaceOrientation.NORTH);
    adjacencies_[(int)FaceOrientation.SOUTH] = new FaceAdjacency(FaceOrientation.SOUTH);
    adjacencies_[(int)FaceOrientation.EAST] = new FaceAdjacency(FaceOrientation.EAST);
    adjacencies_[(int)FaceOrientation.WEST] = new FaceAdjacency(FaceOrientation.WEST);
    adjacencies_[(int)FaceOrientation.TOP] = new FaceAdjacency(FaceOrientation.TOP);
    adjacencies_[(int)FaceOrientation.BOTTOM] = new FaceAdjacency(FaceOrientation.BOTTOM);
  }
 
  public bool Match(FaceAdjacency other_face) {
    // TODO
    return false;
  }

  private void CalculateAdjacencies() {
    Edge[] outer_edges = BuildManifoldEdges(GetComponent<MeshFilter>().mesh);

    foreach (Edge edge in outer_edges) {
      FaceOrientation[] involved_faces = GetFaceOrientationOfEdge(edge);
      foreach(FaceOrientation face_orientation in involved_faces) {
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

    if (faces.Count == 0) {
      faces.Add(FaceOrientation.NONE);

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
