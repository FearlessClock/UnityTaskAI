﻿using Pieter.NavMesh;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadrent
{
    private List<List<RoomInformation>> quad = new List<List<RoomInformation>>();

    public int JaggedCount(int i)
    {
        return quad[i].Count;
    }
    public int Count => quad.Count;

    private Vector2Int offset = new Vector2Int(0, 0);

    public Quadrent(int xOffset, int yOffset)
    {
        offset = new Vector2Int(xOffset, yOffset);
    }

    public void IncreaseQuadrentWidth(int newSize)
    {
        int initSize = quad.Count;
        for (int i = 0; i < newSize - initSize; i++)
        {
            quad.Add(new List<RoomInformation>());
        }
    }

    public void IncreaseQuadrentHeight(int column, int height)
    {
        quad[column].AddRange(new RoomInformation[height - quad[column].Count]);
    }

    public RoomInformation At(int i, int j)
    {
        return quad[i][j];
    }

    public void At(int i, int j, RoomInformation roomAtGridPoint)
    {
        quad[i][j] = roomAtGridPoint ;
    }
}
public class GridWorldMap
{
    /*                  |
    QuadrentUpperLeft   | QuadrentUpperRight
                        |
             -------------------------
                        |
    QuadrentLowerLeft   | QuadrentLowerRight
                        |
    */
    private Quadrent quadrentUpperRight = new Quadrent(0, 0);
    private Quadrent quadrentUpperLeft = new Quadrent(1, 0);
    private Quadrent quadrentLowerRight = new Quadrent(0, 1);
    private Quadrent quadrentLowerLeft = new Quadrent(1, 1);

    public int tileSize = 1;
    public GridWorldMap(int tileSize)
    {
        this.tileSize = tileSize;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="extents"></param>
    /// <param name="angle"></param>
    /// <param name="corner">Direction that the corner is in, has to be +-1</param>
    /// <returns></returns>
    private Vector2Int CalculateVectorRotationAroundPoint(Vector3 center, Vector3 extents, float angle, Vector2 corner)
    {
        angle = angle * Mathf.Deg2Rad;
        return new Vector2Int(Mathf.RoundToInt((center.x + corner.x * (extents.x / 2 * Mathf.Cos(angle) - (extents.z / 2 * Mathf.Sin(angle))))),
                              Mathf.RoundToInt((center.z + corner.y * (extents.x / 2 * Mathf.Sin(angle) + (extents.z / 2 * Mathf.Cos(angle))))));
    }

    public void AddRectangle(Vector2Int lowerLeft, Vector2Int size, RoomInformation room)
    {
        Vector2Int upperLeft = new Vector2Int(lowerLeft.x, lowerLeft.y + size.y);
        Vector2Int lowerRight = new Vector2Int(lowerLeft.x + size.x, lowerLeft.y);

        // Cut the rectangle into 4 smaller squares that I can fit into the quadrents
        if (upperLeft.x < 0 && upperLeft.y >= 0)
        {
            RectInt rect1 = new RectInt(Mathf.Abs(upperLeft.x), Mathf.Abs(Mathf.Max(lowerRight.y, 0)), -Mathf.Abs(Mathf.Min(lowerRight.x, 0) - upperLeft.x), Mathf.Abs(upperLeft.y - Mathf.Max(lowerRight.y, 0)));
            UpdateQuadrentSize(quadrentUpperLeft, rect1);
            UpdateQuadrentAvailablity(quadrentUpperLeft, rect1, room);
        }

        if (lowerRight.x >= 0 && upperLeft.y >= 0)
        {
            RectInt rect2 = new RectInt(Mathf.Max(upperLeft.x, 0), Mathf.Max(lowerRight.y, 0), lowerRight.x - Mathf.Max(upperLeft.x, 0), upperLeft.y - Mathf.Max(lowerRight.y, 0));
            UpdateQuadrentSize(quadrentUpperRight, rect2);
            UpdateQuadrentAvailablity(quadrentUpperRight, rect2, room);
        }
        if (upperLeft.x < 0 && lowerRight.y < 0)
        {
            RectInt rect3 = new RectInt(Mathf.Abs(upperLeft.x), Mathf.Abs(lowerRight.y), -Mathf.Abs(Mathf.Min(lowerRight.x, 0) - upperLeft.x), -Mathf.Abs(Mathf.Min(upperLeft.y, 0) - lowerRight.y));
            UpdateQuadrentSize(quadrentLowerLeft, rect3);
            UpdateQuadrentAvailablity(quadrentLowerLeft, rect3, room);
        }
        if (lowerRight.x >= 0 && lowerRight.y < 0)
        {
            RectInt rect4 = new RectInt(Mathf.Abs(Mathf.Max(upperLeft.x, 0)), Mathf.Abs(lowerRight.y), Mathf.Abs(lowerRight.x - Mathf.Max(upperLeft.x, 0)), -Mathf.Abs(Mathf.Min(upperLeft.y, 0) - Mathf.Min(lowerRight.y, 0)));
            UpdateQuadrentSize(quadrentLowerRight, rect4);

            UpdateQuadrentAvailablity(quadrentLowerRight, rect4, room);
        }
    }

    public RoomInformation At(Vector2 pos)
    {
        Vector2Int adjustedPos;
        Quadrent quad = GetQuadForPosition(pos, out adjustedPos);
        if (quad.Count > Mathf.Abs(adjustedPos.x) && quad.JaggedCount(Mathf.Abs(adjustedPos.x)) > Mathf.Abs(adjustedPos.y))
        {
            return quad.At(Mathf.Abs(adjustedPos.x), Mathf.Abs(adjustedPos.y));
        }
        return null;
    }

    private Quadrent GetQuadForPosition(Vector2 position, out Vector2Int updatedPosition)
    {
        updatedPosition = new Vector2Int() ;
        if (position.x < 0 && position.y >= 0)
        {
            updatedPosition = new Vector2Int((int)(position.x + 1), (int)position.y);
            return quadrentUpperLeft;
        }
        else if (position.x >= 0 && position.y >= 0)
        {
            updatedPosition = new Vector2Int((int)position.x, (int)position.y);
            return quadrentUpperRight;
        }
        else if (position.x < 0 && position.y < 0)
        {
            float X = position.x;
            float Y = position.y;
            Y = position.y + 1;
            X = position.x + 1;
            updatedPosition = new Vector2Int((int)X, (int)Y);
            return quadrentLowerLeft;
        }
        else if (position.x >= 0 && position.y < 0)
        {
            float X = position.x;
            float Y = position.y;
            Y = position.y + 1;
            updatedPosition = new Vector2Int((int)X, (int)Y);
            return quadrentLowerRight;
        }
        return null;
    }

    private void UpdateQuadrentAvailablity(Quadrent quad, RectInt rect, RoomInformation roomAtGridPoint)
    {
        for (int i = rect.xMin; i < rect.xMax; i++)
        {
            for (int j = rect.yMin; j < rect.yMax; j++)
            {
                quad.At(i, j, roomAtGridPoint);
            }
        }
    }

    private bool CheckForBlockedSquares(Quadrent quad, RectInt rect)
    {
        for (int i = rect.xMin; i < rect.xMax; i++)
        {
            for (int j = rect.yMin; j < rect.yMax; j++)
            {
                if (quad.Count > i && quad.JaggedCount(i) > j && quad.At(i,j) != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void UpdateQuadrentSize(Quadrent quad, RectInt rect)
    {
        if (quad.Count <= rect.xMax)
        {
            quad.IncreaseQuadrentWidth(rect.xMax);
        }

        for (int i = rect.xMin; i < rect.xMax; i++)
        {
            if (quad.JaggedCount(i) <= rect.yMax)
            {
                quad.IncreaseQuadrentHeight(i, rect.yMax);
            }
        }
    }


    public void DrawDebug(Color color)
    {
        Gizmos.color = color;
        DebugDrawQuadrent(quadrentLowerLeft, Color.red, new Vector2(-1, 1));
        DebugDrawQuadrent(quadrentLowerRight, Color.green, new Vector2(1, 1));
        DebugDrawQuadrent(quadrentUpperLeft, Color.cyan, new Vector2(-1, -1));
        DebugDrawQuadrent(quadrentUpperRight, Color.yellow, new Vector2(1, -1));
    }

    public bool IsGridSpaceFree(Vector2Int startingCell, Vector2Int size)
    {
        Vector2Int lowerLeft = startingCell;
        Vector2Int upperRight = startingCell + (size);
        Debug.DrawLine(new Vector3(lowerLeft.x, 0, lowerLeft.y), new Vector3(upperRight.x, 0, upperRight.y), Color.green, 2);

        if (lowerLeft.x < 0 && upperRight.y >= 0)
        {
            // Transforming from world space to quadrent space
            RectInt rect = new RectInt(Mathf.Abs(lowerLeft.x), Mathf.Abs(Mathf.Max(lowerLeft.y, 0)), -Mathf.Abs(Mathf.Min(upperRight.x, 0) - lowerLeft.x), Mathf.Abs(upperRight.y - Mathf.Max(lowerLeft.y, 0)));

            Debug.DrawLine(new Vector3(rect.min.x, 0, rect.min.y), new Vector3(rect.max.x, 0, rect.max.y), Color.red, 2);
            bool res = CheckForBlockedSquares(quadrentUpperLeft, rect);
            if (res)
            {
                return false;
            }
        }

        if (upperRight.x >= 0 && upperRight.y >= 0)
        {
            RectInt rect = new RectInt(Mathf.Max(lowerLeft.x, 0), Mathf.Max(lowerLeft.y, 0), upperRight.x - Mathf.Max(lowerLeft.x, 0), upperRight.y - Mathf.Max(lowerLeft.y, 0));

            Debug.DrawLine(new Vector3(rect.min.x, 0, rect.min.y), new Vector3(rect.max.x, 0, rect.max.y), Color.magenta, 2);
            bool res = CheckForBlockedSquares(quadrentUpperRight, rect);
            if (res)
            {
                return false;
            }
        }

        if (lowerLeft.x < 0 && lowerLeft.y < 0)
        {
            RectInt rect = new RectInt(Mathf.Abs(lowerLeft.x), Mathf.Abs(lowerLeft.y), -Mathf.Abs(Mathf.Min(upperRight.x, 0) - lowerLeft.x), -Mathf.Abs(Mathf.Min(upperRight.y, 0) - lowerLeft.y));

            Debug.DrawLine(new Vector3(rect.min.x, 0, rect.min.y), new Vector3(rect.max.x, 0, rect.max.y), Color.yellow, 2);
            bool res = CheckForBlockedSquares(quadrentLowerLeft, rect);
            if (res)
            {
                return false;
            }
        }
        if (upperRight.x >= 0 && lowerLeft.y < 0)
        {
            RectInt rect = new RectInt(Mathf.Abs(Mathf.Max(lowerLeft.x, 0)), Mathf.Abs(lowerLeft.y), Mathf.Abs(upperRight.x - Mathf.Max(lowerLeft.x, 0)), -Mathf.Abs(Mathf.Min(upperRight.y, 0) - Mathf.Min(lowerLeft.y, 0)));

            Debug.DrawLine(new Vector3(rect.min.x, 0, rect.min.y), new Vector3(rect.max.x, 0, rect.max.y), Color.blue, 2);
            bool res = CheckForBlockedSquares(quadrentLowerRight, rect);
            if (res)
            {
                return false;
            }
        }
        return true;
    }
    private void DebugDrawQuadrent(Quadrent quad, Color color, Vector2 inversionMatrix)
    {
        for (int i = 0; i < quad.Count; i++)
        {
            for (int j = 0; j < quad.JaggedCount(i); j++)
            {
                if (i == 0 && j == 0)
                {
                    //Gizmos.color = Color.gray;
                }
                else if (i == quad.Count - 1 && j == quad.JaggedCount(i) - 1)
                {
                    //Gizmos.color = Color.blue;
                }
                else
                {
                    if (quad.At(i,j) == null)
                    {
                        Gizmos.color = Color.magenta;
                    }
                    else
                    {
                        Gizmos.color = color;
                    }
                }
                Gizmos.DrawWireCube(new Vector3(((i+ Mathf.Min(inversionMatrix.x, 0)*-1) * tileSize) * inversionMatrix.x + (0.5f * tileSize), 0, (-(j+ Mathf.Max(inversionMatrix.y, 0)) * tileSize) * inversionMatrix.y + (0.5f * tileSize)), Vector3.one * tileSize);
            }
        }
    }
}
