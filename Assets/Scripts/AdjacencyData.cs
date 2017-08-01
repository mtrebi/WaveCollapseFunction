using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FaceOrientation {
  NORTH,
  WEST,
  SOUTH,
  EAST,
  TOP,
  BOTTOM,
  NONE
}

public class Edge {
  public Vector3 v1,
                 v2;
  // TODO face index!
  public int[] faceIndex = new int[2];

  public override bool Equals(object obj) {
    var item = obj as Edge;
    if (item == null) return false;

    return this.v1.Equals(item.v1) && this.v2.Equals(item.v2)
           || this.v1.Equals(item.v2) && this.v2.Equals(item.v1);
  }

  public override int GetHashCode() {
    return v1.GetHashCode() + v2.GetHashCode();
  }
}

public class FaceAdjacency {
  private FaceOrientation face_orientation_;
  private List<Edge> edges_;

  public FaceAdjacency(FaceOrientation face_orientation) {
    face_orientation_ = face_orientation;
    edges_ = new List<Edge>();
  }

  // TODO Left to right, bottom to top
  public void AddEdge(Edge edge) {
    //Edge sorted_edge = SortEdge(edge);


    // Calculate dimensions - what means to be smaller


    edges_.Add(edge);
  }
  /*
  private Edge SortEdge(Edge edge) {
    float left, up;
    CalculateInvolvedDimensions(edge, out left, out up);

    if 

    Edge sorted_edge;

    // Calculate smaller vertex 
    // Calculate greater vertex

    // Create new Edge from smaller to greater
    return sorted_edge;
  }
  */
  /*
  private void a(Edge edge) {
    Vector3 smaller_vertex, greater_vertex;
    switch (face_orientation_) {
      case FaceOrientation.TOP:
        //z, y
        break;
      case FaceOrientation.BOTTOM:
        left = edge.v1.x;
        up = edge.v1.z;
        break;
      case FaceOrientation.NORTH:
        // z, y but comparision signs should be changed on Z (or multiply by -1 one side)
      case FaceOrientation.SOUTH:
        // z, y
        if (edge.v1.y < edge.v2.y) { //d2
          smaller_vertex = edge.v1;
          greater_vertex = edge.v2;
        }
        else if (edge.v1.y > edge.v2.y) { // d2
          smaller_vertex = edge.v2;
          greater_vertex = edge.v1;
        }
        else {
          if (edge.v1.z < edge.v2.z) { //d1
            smaller_vertex = edge.v1;
            greater_vertex = edge.v2;
          }else if (edge.v1.z > edge.v2.z) { //d1
            smaller_vertex = edge.v2;
            greater_vertex = edge.v1;
          }
        }
        break;
      case FaceOrientation.EAST:
      case FaceOrientation.WEST:
        left = edge.v1.x;
        up = edge.v1.y;
        break;
    }
  }
  }
  */
  /*
  private void CalculateInvolvedDimensions(Edge edge, out float left, out float up) {
    left = 0;
    up = 0;
    switch (face_orientation_) {
      case FaceOrientation.TOP:
      case FaceOrientation.BOTTOM:
        left = edge.v1.x;
        up = edge.v1.z;
        break;
      case FaceOrientation.NORTH:
      case FaceOrientation.SOUTH:
        left = edge.v1.z;
        up = edge.v1.y;
        break;
      case FaceOrientation.EAST:
      case FaceOrientation.WEST:
        left = edge.v1.x;
        up = edge.v1.y;
        break;
    }
  }
  */
  public void Draw(Color color) {
    foreach(Edge edge in edges_) {
      Debug.DrawLine(edge.v1, edge.v2, color, 1000.0f, false);
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
    adjacencies_[(int)FaceOrientation.BOTTOM].Draw(Color.red);
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
 
  private void CalculateAdjacencies() {
    Edge[] outer_edges = BuildManifoldEdges(GetComponent<MeshFilter>().mesh);

    foreach(Edge edge in outer_edges) {
      FaceOrientation[] involved_faces = GetFaceOrientationOfEdge(edge);
      foreach(FaceOrientation face_orientation in involved_faces) {
        adjacencies_[(int)face_orientation].AddEdge(edge);
      }
    }
  }

  /// <summary>
  /// Checks on which side of the imaginary cube the edge lays
  /// </summary>
  /// <param name="edge"> Edge to check </param>
  /// <returns> Returns on which side the edge lays. Can return None if the edge doesnt lay on any side </returns>
  private FaceOrientation[] GetFaceOrientationOfEdge(Edge edge) {
    List<FaceOrientation> faces = new List<FaceOrientation>();

    if (edge.v1.z == edge.v2.z && edge.v1.z == 0.5) {
      faces.Add(FaceOrientation.SOUTH);
    }
    else if (edge.v1.z== edge.v2.z && edge.v1.z == -0.5) {
      faces.Add(FaceOrientation.NORTH);
    }

    if (edge.v1.x == edge.v2.x && edge.v1.x == 0.5) {
      faces.Add(FaceOrientation.WEST);
    }
    else if (edge.v1.x == edge.v2.x && edge.v1.x == -0.5) {
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


    /*

    // We only want edges that connect to a single triangle
    ArrayList culledEdges = new ArrayList();
    foreach (Edge edge in edges) {
      if (edge.faceIndex[0] == edge.faceIndex[1]) {
        culledEdges.Add(edge);
      }
    }

    return culledEdges.ToArray(typeof(Edge)) as Edge[];
    */
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
          Edge newEdge = new Edge();
          newEdge.v1 = mesh.vertices[i1];
          newEdge.v2 = mesh.vertices[i2];
          newEdge.faceIndex[0] = a;
          newEdge.faceIndex[1] = a;
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
            Edge newEdge = new Edge();
            newEdge.v1 = mesh.vertices[i1];
            newEdge.v2 = mesh.vertices[i2];
            newEdge.faceIndex[0] = a;
            newEdge.faceIndex[1] = a;
            edgeArray[edgeCount] = newEdge;
            edgeCount++;
          }
        }
        i1 = i2;
      }
    }

    Edge[] compactedEdges = new Edge[edgeCount];
    for (int e = 0; e < edgeCount; e++)
      compactedEdges[e] = edgeArray[e];

    return compactedEdges;
  }
}
