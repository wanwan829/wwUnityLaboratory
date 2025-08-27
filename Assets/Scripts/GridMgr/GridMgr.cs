using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMgr : MonoBehaviour
{
    public int Width;
    public int Height;
    public float NodeSize;
    public NodeComponent nodeComponent;
    public AstarNodeMgr nodeMgr;
    public GridMgr(int width, int height, float nodeSize, NodeComponent nodePrefab, Transform startTrans)
    {
        Width = width;
        Height = height;
        NodeSize = nodeSize;
        nodeComponent = nodePrefab;
        nodeMgr = new AstarNodeMgr(Width, Height);


    }


    public GridMgr(AStarGridData astarGameData)
    {
        Width = astarGameData.Width;
        Height = astarGameData.Height;
        NodeSize = astarGameData.NodeSize;
        nodeComponent = astarGameData.NodeCompot;
    }

    public NodeComponent[,] Grids { get; private set; }
    public void GenerateGrid(Transform parent)
    {

        Grids = new NodeComponent[Width, Height];
        Vector3 startPos = parent.position;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 pos = new Vector3();
                pos = new Vector2(startPos.x + x * NodeSize, startPos.y + y * NodeSize);
                var nodeObj = GameObject.Instantiate(this.nodeComponent.gameObject, pos, Quaternion.identity, parent);
                nodeObj.name = $"{x}_{y}";
                var nodeComponent = nodeObj.GetComponent<NodeComponent>();
                nodeComponent.node.Position = new Vector2Int(x, y);
                nodeComponent.node.NodeType = NodeType.Normal;
                //nodeComponent.node.F = 10000;

                nodeComponent.node.IsWalkable = true;
                Grids[x, y] = nodeComponent;
                nodeMgr.Nodes[x, y] = nodeComponent.node;


            }
        }
    }


    public void SetAstarObsData(List<Vector2Int> obsData)
    {
        for (int i = 0; i < obsData.Count; i++)
        {
            if (Grids[obsData[i].x, obsData[i].y])
            {
                Grids[obsData[i].x, obsData[i].y].node.IsWalkable = false;
            }
        }

    }

    public NodeComponent GetNodeComponent(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height) ? Grids[pos.x, pos.y] : null;
    }


    public Node GetNode(Vector2Int pos)
    {
        return (pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height) ? Grids[pos.x, pos.y].node : null;
    }

    public void ResetNodeStateExceptObs()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                NodeComponent nodeComponent = GetNodeComponent(new Vector2Int(i, j));
                if (nodeComponent != null && nodeComponent.node.IsWalkable)
                {
                    nodeComponent.node.Parent = null;
                    nodeComponent.node.IsWalkable = true;
                    nodeComponent.node.NodeType = NodeType.Normal;
                }

            }
        }
    }


    public void ResetNodeCompotStateExceptObs()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                NodeComponent nodeComponent = GetNodeComponent(new Vector2Int(i, j));
                if (nodeComponent != null && nodeComponent.node.IsWalkable)
                {
                    nodeComponent.Clear();
                }
            }
        }
    }

    public void ResetAllNodes()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                NodeComponent nodeComponent = GetNodeComponent(new Vector2Int(i, j));
                nodeComponent.node.Parent = null;
                nodeComponent.node.IsWalkable = true;
                nodeComponent.node.F = 10000;
                nodeComponent.SetAsDefault();
                nodeComponent.node.NodeType = NodeType.Normal;
            }
        }
    }

    public void ResetAllCompareNodes()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                NodeComponent nodeComponent = GetNodeComponent(new Vector2Int(i, j));
                nodeComponent.node.Parent = null;
                nodeComponent.node.IsWalkable = true;
                nodeComponent.node.F = 10000;
                if (nodeComponent.node.NodeType != NodeType.Obs)
                {
                    nodeComponent.SetAsDefault();
                }
                nodeComponent.node.NodeType = NodeType.Normal;
            }
        }
    }


    public void ResetAllCompots()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                NodeComponent nodeComponent = GetNodeComponent(new Vector2Int(i, j));
                if (nodeComponent != null)
                {
                    nodeComponent.Clear();
                }
            }
        }
    }
}
