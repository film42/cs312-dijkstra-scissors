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
  public override void FindSegmentation(IList<Point> points, Pen pen)
  {
    // this is the entry point for this class when the button is clicked for 
    // segmenting the image using the simple greedy algorithm. 
    // the points
    if (Image == null) throw new InvalidOperationException("Set Image property first.");
    if (points.Count == 1) return;

    Graphics g = Graphics.FromImage(Overlay); 

    Point nextPoint;
    Point currenctPoint = points[0];
    settled.Add(currenctPoint);

    for (int i = 1; i < points.Count; i++) {
      nextPoint = points[i];
      while (currenctPoint != nextPoint && !IsEdgePoint(currenctPoint)) {
        g.FillRectangle(pen.Brush, currenctPoint.X, currenctPoint.Y, 1, 1);
        currenctPoint = GetSmallestNeighborPoint(currenctPoint);
        settled.Add(currenctPoint);
      }
      currenctPoint = nextPoint;
    }

    // Clear
    settled = new List<Point>();
  }

    private Point GetSmallestNeighborPoint(Point p) {
      List<Point> neighbors = new List<Point>();

      // Oriented points: N, E, S, W
      neighbors.Add( new Point(p.X, p.Y + 1) );
      neighbors.Add( new Point(p.X, p.Y - 1) );
      neighbors.Add( new Point(p.X + 1, p.Y) );
      neighbors.Add( new Point(p.X - 1, p.Y) );

      // See which one is smallest
      Point smallestNeighbor = new Point(-1, -1);
      int smallestValue = int.MaxValue;
      foreach (Point n in neighbors) { 
        int value = GetPixelWeight(n);
        if (!IsEdgePoint(n) && !IsPointSettled(n) && value < smallestValue) {
          smallestValue = value;
          smallestNeighbor = n;
        }
      }

      // Return
      return smallestNeighbor;
    }

    private bool IsPointSettled(Point p) {
      var set = settled.Where(q => q.X == p.X && q.Y == p.Y);
      return set.Any();
    }

    private bool IsEdgePoint(Point p) {
      return (p.X <= 1 || p.Y <= 1 || p.Y >= Image.Bitmap.Width - 1 || p.Y >= Image.Bitmap.Height - 1) ;
    }
  }
}
