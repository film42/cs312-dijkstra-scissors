using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.Linq; 

namespace VisualIntelligentScissors {
  public class DijkstraScissors : Scissors {

    public DijkstraScissors() { }

    /// <summary>
    /// constructor for intelligent scissors. 
    /// </summary>
    /// <param name="image">the image you are oging to segment.  has methods for getting gradient information</param>
    /// <param name="overlay">an overlay on which you can draw stuff by setting pixels.</param>
    public DijkstraScissors(GrayBitmap image, Bitmap overlay) : base(image, overlay) { }

    /// <summary>
    /// this is the class you implement in CS 312. 
    /// </summary>
    /// <param name="points">these are the segmentation points from the pgm file.</param>
    /// <param name="pen">this is a pen you can use to draw on the overlay</param>
    public override void FindSegmentation(IList<Point> points, Pen pen) {

      // this is the entry point for this class when the button is clicked
      // to do the image segmentation with intelligent scissors.
      Program.MainForm.RefreshImage();

      if (Image == null) throw new InvalidOperationException("Set Image property first.");
      if (points.Count == 1) return;

      // Ensure the points connect through
      points.Add(points[0]);

      // Setup graphics
      Graphics g = Graphics.FromImage(Overlay);

      // Declare vars and set current to first point
      Node shortestPath = null;
      Point nextPoint;
      Point currentPoint = points[0];

      for (int i = 1; i < points.Count; i++) {
        nextPoint = points[i];
        // Create some vars
        PrioQueue q = new PrioQueue();
        //PriorityQueue<Node> q = new PriorityQueue<Node>();
        var results = new Dictionary<int, Dictionary<int, Node>>();
        int currentWeight = GetPixelWeight(currentPoint);
        bool foundGoal = false;
        // Enqueue to make it past the first loop; weight of 0
        Node start = new Node(null, currentPoint, currentWeight);
        q.Enqueue(start, 0);
        // Settle the start point
        AddNodeToResults(ref results, start);

        while (!q.IsEmpty() && !foundGoal) {
          // Pop the lowest cost node
          Node v = (Node)q.Dequeue();
          // Iterate over each child
          foreach (Point p in GetNeighbors(v.Current)) {
            // If destination point, stop.
            if (p.X == nextPoint.X && p.Y == nextPoint.Y) {
              foundGoal = true;
              int weight = v.Total + GetPixelWeight(p);
              shortestPath = new Node(v, p, weight);
            }
            // If it's not a visited node, or queued for exploration, do so now
            else if (!InResults(ref results, p)) {
              // Calculate new weight and enqueue a new Node
              int weight = v.Total + GetPixelWeight(p);
              Node c = new Node(v, p, weight);
              // Enqueue the new node
              q.Enqueue(c, weight);
              // Settle the new node right now
              AddNodeToResults(ref results, c);
              //g.FillRectangle(pen.Brush, p.X, p.Y, 1, 1);
            }
          }
        
        }

        // Now we've found a point, so we print it
        while (shortestPath != null) {
          // For each point draw a dot
          Point c = shortestPath.Current;
          g.FillRectangle(pen.Brush, c.X, c.Y, 1, 1);
          // Iterate
          shortestPath = shortestPath.Parent;
        }

        // Iterate to the next Point
        currentPoint = nextPoint;
      }
    }

    public void AddNodeToResults(ref Dictionary<int, Dictionary<int, Node>> results, Node node) { 
      Point pt = node.Current;
      // If the Y key doesn't exist, make a new Dictionary and settle it
      if (!results.ContainsKey(pt.Y))
        results.Add(pt.Y, new Dictionary<int,Node>());

      // Otherwise just settle it
      results[pt.Y][pt.X] = node;
    }

    public bool InResults(ref Dictionary<int, Dictionary<int, Node>> results, Point pt) {
      // If the key exists for both, it's found
      if (results.ContainsKey(pt.Y))
        if (results[pt.Y].ContainsKey(pt.X))
          return true;

      // Otherwise, not found
      return false;
    }

    private bool IsEdgePoint(Point p) {
      return (p.X <= 1 || p.Y <= 1 || p.Y >= Image.Bitmap.Width - 1 || p.Y >= Image.Bitmap.Height - 1);
    }

    public List<Point> GetNeighbors(Point p) {
      List<Point> neighbors = new List<Point>();

      // Oriented points: N, S, E, W
      if (!IsEdgePoint(new Point(p.X, p.Y - 1)))
        neighbors.Add(new Point(p.X, p.Y - 1));

      if (!IsEdgePoint(new Point(p.X, p.Y + 1)))
        neighbors.Add(new Point(p.X, p.Y + 1));

      if (!IsEdgePoint(new Point(p.X + 1, p.Y)))
        neighbors.Add(new Point(p.X + 1, p.Y));

      if (!IsEdgePoint(new Point(p.X - 1, p.Y)))
        neighbors.Add(new Point(p.X - 1, p.Y));

      return neighbors;
    }
  }

  // Node class to be used in Priority Queue
  // Also used to chain our paths for easier drawing
  public class Node {

    public Node Parent;
    public Point Current;
    public int Total;

    public Node(Node Parent, Point Current, int Total) {
      this.Parent = Parent;
      this.Current = Current;
      this.Total = Total;
    }

  }

  //
  // CREDIT: http://stackoverflow.com/a/4994931/1457934
  //
  public class PrioQueue {
    int total_size;
    SortedDictionary<int, Queue> storage;

    public PrioQueue() {
      this.storage = new SortedDictionary<int, Queue>();
      this.total_size = 0;
    }

    public bool IsEmpty() {
      return (total_size == 0);
    }

    public object Dequeue() {
      if (IsEmpty()) {
        throw new Exception("Please check that priorityQueue is not empty before dequeing");
      } else {
        var kv = storage.First();
        Queue q = kv.Value;
        object deq = q.Dequeue();

        if (q.Count == 0)
          storage.Remove(kv.Key);

        return deq;
      }
    }

    public void Enqueue(object item, int prio) {
      if (!storage.ContainsKey(prio)) {
        storage.Add(prio, new Queue());
      }
      storage[prio].Enqueue(item);
      total_size++;

    }
  }
}
