using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace VisualIntelligentScissors {
  public class SimpleScissors : Scissors {

    public SimpleScissors() { }

    private List<Point> settled = new List<Point>();

    /// <summary>
    /// constructor for SimpleScissors. 
    /// </summary>
    /// <param name="image">the image you are going to segment including methods for getting gradients.</param>
    /// <param name="overlay">a bitmap on which you can draw stuff.</param>
    public SimpleScissors(GrayBitmap image, Bitmap overlay) : base(image, overlay) { }

    // this is a class you need to implement in CS 312. 

    /// <summary>
    ///  this is the class to implement for CS 312. 
    /// </summary>
    /// <param name="points">the list of segmentation points parsed from the pgm file</param>
    /// <param name="pen">a pen for writing on the overlay if you want to use it.</param>
    public override void FindSegmentation(IList<Point> points, Pen pen) {
      // this is the entry point for this class when the button is clicked for 
      // segmenting the image using the simple greedy algorithm. 
      // the points
      if (Image == null) throw new InvalidOperationException("Set Image property first.");
      if (points.Count <= 1) return;

      // Ensure the points cycle
      points.Add(points[0]);

      // Create our graphics object
      Graphics g = Graphics.FromImage(Overlay); 

      // Declare next point and set first point to settled set
      Point nextPoint;
      Point currentPoint = points[0];
      settled.Add(currentPoint);

      // Start from point 2 and iterate until the end
      for (int i = 1; i < points.Count; i++) {
        // Set next point for reference
        nextPoint = points[i];

        // While we haven't found the next point and haven't hit a border
        while (currentPoint != nextPoint && !IsEdgePoint(currentPoint)) {
          // Draw the point
          g.FillRectangle(pen.Brush, currentPoint.X, currentPoint.Y, 1, 1);
          // Set current point to be the smallest neighbor
          currentPoint = GetSmallestNeighborPoint(currentPoint);
          // Settle the new point
          settled.Add(currentPoint);
        }
        // Iterate to the next point
        currentPoint = nextPoint;
      }

      // Clear
      settled = new List<Point>();
    }

    private Point GetSmallestNeighborPoint(Point p) {
      List<Point> neighbors = new List<Point>();

      // Oriented points: N, S, E, W
      neighbors.Add( new Point(p.X, p.Y - 1) );
      neighbors.Add( new Point(p.X, p.Y + 1) );
      neighbors.Add( new Point(p.X + 1, p.Y) );
      neighbors.Add( new Point(p.X - 1, p.Y) );

      // See which one is smallest
      Point smallestNeighbor = new Point(-1, -1);
      int smallestValue = int.MaxValue;
      // Iterate through neighbors
      foreach (Point n in neighbors) { 
        int value = GetPixelWeight(n);
        // Get the smallest point here
        if (!IsEdgePoint(n) && !IsPointSettled(n) && value < smallestValue) {
          smallestValue = value;
          smallestNeighbor = n;
        }
      }

      // Return smallest particular neighbor
      return smallestNeighbor;
    }

    private bool IsPointSettled(Point p) {
      // Search settled list for a point
      var set = settled.Where(q => q.X == p.X && q.Y == p.Y);
      return set.Any();
    }

    private bool IsEdgePoint(Point p) {
      // Check if a point is beyond the bounds
      return (p.X <= 1 || p.Y <= 1 || p.Y >= Image.Bitmap.Width - 1 || p.Y >= Image.Bitmap.Height - 1);
    }
  }
}
