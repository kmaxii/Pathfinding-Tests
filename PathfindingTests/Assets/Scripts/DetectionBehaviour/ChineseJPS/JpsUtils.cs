using System.Collections.Generic;
using UnityEngine;

public interface IGrid
{
    bool IsMovable(Vector2Int pos); // Check if the target position is movable
    bool IsWall(Vector2Int pos); // Check if the target position is a wall or not movable
}

public static class JpsUtils
{
    public static readonly Vector2Int Left = Vector2Int.left;
    public static readonly Vector2Int Right = Vector2Int.right;
    public static readonly Vector2Int Up = Vector2Int.up;
    public static readonly Vector2Int Down = Vector2Int.down;
    public static readonly Vector2Int UpRight = Vector2Int.one;
    public static readonly Vector2Int UpLeft = new Vector2Int(-1, 1);
    public static readonly Vector2Int DownRight = new Vector2Int(1, -1);
    public static readonly Vector2Int DownLeft = new Vector2Int(-1, -1);
    public static Dictionary<Vector2Int, Vector2Int[]> verticalDirLut;

    public static void Init()
    {
        verticalDirLut = new Dictionary<Vector2Int, Vector2Int[]>();
        Vector2Int[] horizontalLines = new Vector2Int[] {Left, Right};
        Vector2Int[] verticalLines = new Vector2Int[] {Up, Down};
        verticalDirLut.Add(Left, verticalLines);
        verticalDirLut.Add(Right, verticalLines);
        verticalDirLut.Add(Up, horizontalLines);
        verticalDirLut.Add(Down, horizontalLines);
    }

    /// <summary> Determine if the current direction is a straight line direction </summary>
    public static bool IsLineDireaction(Vector2Int direaction)
    {
        return direaction.x * direaction.y == 0;
    }

    public static int EucDistance(Vector2Int p1, Vector2Int p2)
    {
        /* Euclidean distance */

        int dx = p1.x - p2.x;
        int dy = p1.y - p2.y;
        return dx * dx + dy * dy;
    }
}