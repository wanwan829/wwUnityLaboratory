using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node: IComparable<Node>
{
    public Vector2Int Position { get; set; }
    public bool IsWalkable { get; set; }
    public bool IsWall{ get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public Node Parent { get; set; }      
    public NodeType NodeType { get; set; }

    public int CompareTo(Node other)
    {
        return F.CompareTo(other?.F);
    }
}

