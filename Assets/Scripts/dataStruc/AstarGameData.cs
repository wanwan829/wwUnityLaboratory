using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGridData
{

    public AStarGridData(NodeComponent _NodeCompot, int _Width, int _Height, int _NodeSize,string _ObsDataPath)
    {
        this.NodeCompot = _NodeCompot;
        this.Width = _Width;
        this.Height = _Height;
        this.NodeSize = _NodeSize;
        this.ObsDataPath = _ObsDataPath;


    }
    public NodeComponent NodeCompot;
    public Transform GridBirthTrans;
    public int Width;
    public int Height;
    public int NodeSize;
    public string ObsDataPath;

}


public class AStarCalculData
{
    public Node StartNode;
    public Node EndNode;
    public List<Vector2Int> obsData;

}
