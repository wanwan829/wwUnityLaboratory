using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarNodeMgr 
{
    public int Width;
    public int Height;
    public Node[,] Nodes;

    public AstarNodeMgr(int width, int height)
    {
        Width = width;
        Height = height;
        Nodes = new Node[width,height];
    }

    /// <summary>
    /// 是否是可行走点
    /// </summary>
    /// <param name="jpsNode"></param>
    /// <returns></returns>
    public bool IsWalkable(int x ,int y)
    {
        //Debug.Log($"x{x} y{y} width{Width} height{Height}");
        if (x <0||y<0||x+1>Width || y + 1 > Height) return false;
        var node = Nodes[x, y];
        //障碍/超出地图 皆为不可行走
        if (!node.IsWalkable) return false;
        return true;
    }


    /// <summary>
    /// 是否是可行走或墙壁
    /// </summary>
    /// <param name="jpsNode"></param>
    /// <returns></returns>
    public bool IsWalkableOrWall (int x, int y)
    {
        //Debug.Log($"x{x} y{y} width{Width} height{Height}");
        if (x<0||y<0||x + 1 > Width || y + 1 > Height) return false;
        if (x == 0 || x == Width - 1 || y == 0 || y == Width - 1) return false;
        var node = Nodes[x, y];
        //障碍/超出地图 皆为不可行走
        if (!node.IsWalkable) return false;
        return true;
    }

    public Node GetNode(int x,int y)
    {
        if (Nodes[x, y] == null) return null;
        return Nodes[x, y];
    }

}
