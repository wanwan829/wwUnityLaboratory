using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JpsFindBrain : IFindPathAlgor
{

    #region 变量定义
    private int MapWidth;
    private int MapHeight;
    private MiniHeap<Node> openSet;
    private HashSet<Node> closeSet;
    private AstarNodeMgr astarNodeMgr;
    public List<Node> jumpPointArray = new List<Node>();
    public List<Node> jumpPointArray1 = new List<Node>();
    #endregion
    #region 构造函数
    public JpsFindBrain(AstarNodeMgr astarNodeMgr)
    {
        this.astarNodeMgr = astarNodeMgr;
        openSet = new MiniHeap<Node>();
        closeSet = new HashSet<Node>();
    }
    #endregion
    #region 方法调用
    /// <summary>
    /// 寻路接口
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<Node> FindPath(Node start, Node end)
    {
        //清理
        closeSet.Clear();
        while (openSet.Count != 0)
        {
            openSet.Pop();
        }
        Node node = FindRootNode(start, end);
        if (node != null)
        {
            return ReversePath(node);
        }
        Debug.Log("没有找到路径");
        return null;
    }

    /// <summary>
    /// 寻路根节点建立
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    Node FindRootNode(Node start,Node end)
    {
        //计算起点的G.H值，并加入开放列表。
        start.G = 0;
        start.H = CalcH(start, end);
        openSet.Push(start);
        while (openSet.Count != 0)  
        {
            //开放列表泵出最小值
            Node cur = openSet.Pop();
            //关闭列表进行记录
            closeSet.Add(cur);
            //当前节点就是目标点，返回当前节点。
            if (cur == end)
            {
                return cur;
            }
            //获取当前节点邻居列表，即探索方向列表。
            List<Vector2Int> Neighbors = GetNeighbors(cur);
            //遍历邻居列表
            for (int i = 0; i < Neighbors.Count; i++)
            {   
                Vector2Int dir = Neighbors[i] - cur.Position;
                //寻找当前方向跳点
                var jumpPoint = Jump(Neighbors[i], dir.x, dir.y, end);
                //如果找到跳点
                if (jumpPoint != null && !closeSet.Contains(jumpPoint))
                {   
                    //更新跳点的G,H,F，以及Parent
                    jumpPoint.G = cur.G + CalcG(cur, jumpPoint);
                    jumpPoint.H = CalcH(jumpPoint, end);
                    jumpPoint.F = jumpPoint.G + jumpPoint.H;
                    jumpPoint.Parent = cur;
                    //将跳点加入开放列表
                    openSet.Push(jumpPoint);
                    if (jumpPoint.NodeType == NodeType.End)
                    {
                        break;
                    }
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 获取邻居点
    /// </summary>
    /// <param name="node"></param>
    List<Vector2Int> GetNeighbors(Node node)
    {
        List<Vector2Int> directions = new List<Vector2Int>();
        //节点的父节点为空，即第一个节点
        if(node.Parent == null)
        {
            //探索八个方向
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;
                    int xPos = node.Position.x + x;
                    int yPos = node.Position.y + y;
                    if (astarNodeMgr.IsWalkable(xPos,yPos))
                    {
                        directions.Add(new Vector2Int(xPos, yPos));
                    }
                }
            }
            return directions;

        }
        //判断当前节点和父节点的方向关系 横纵/对角
        int finalX = Mathf.Clamp(node.Position.x - node.Parent.Position.x,-1,1);
        int finalY = Mathf.Clamp(node.Position.y - node.Parent.Position.y, -1, 1);
        Vector2Int dPos = new Vector2Int(finalX,finalY);
        //横纵向和对角向分开处理
        if (dPos.x == 0 || dPos.y == 0)
        {
      
            //判断向前方是否可行
            bool forward = astarNodeMgr.IsWalkable(node.Position.x + dPos.x, node.Position.y + dPos.y);
            if (forward)
            {
                directions.Add(new Vector2Int(node.Position.x + dPos.x, node.Position.y + dPos.y));
            }
            //判断是否存在强迫邻居方向的存在性与方向
            List<Vector2Int> forceDirs = GetLineForceNeighbor(node.Position, dPos);
            if (forceDirs.Count > 0)
            {   
                //存在强迫邻居，遍历添加
                for (int i = 0; i < forceDirs.Count; i++)
                {
                    directions.Add(forceDirs[i]);

                }
            }
        }
        else
        {
            //先判断对角向的三个方向是否可行
            bool forward = astarNodeMgr.IsWalkable(node.Position.x + dPos.x, node.Position.y + dPos.y);
            bool xAxis = astarNodeMgr.IsWalkable(node.Position.x + dPos.x, node.Position.y);
            bool yAxis = astarNodeMgr.IsWalkable(node.Position.x, node.Position.y + dPos.y);
            if (forward)
            {
                directions.Add(new Vector2Int(node.Position.x + dPos.x, node.Position.y + dPos.y));
            }
            if (xAxis)
            {
                directions.Add(new Vector2Int(node.Position.x + dPos.x, node.Position.y));
            }
            if (yAxis)
            {
                directions.Add(new Vector2Int(node.Position.x, node.Position.y + dPos.y));
            }
            //判断对角向的强迫邻居是否可行
            List<Vector2Int> forceDirs = GetDiagonalNeighbor(node.Position, dPos);
            if (forceDirs.Count > 0)
            {
                for (int i = 0; i < forceDirs.Count; i++)
                {
                    directions.Add(forceDirs[i]);
                }
            }
        }

        return directions;
    }

    /// <summary>
    /// 寻找跳点
    /// </summary>
    /// <param name="current"></param>
    /// <param name="xDirect"></param>
    /// <param name="yDirect"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    Node Jump(Vector2Int current,int xDirect,int yDirect,Node end)
    {
        if (!astarNodeMgr.IsWalkable(current.x, current.y)) return null;
        if (current.x == end.Position.x && current.y == end.Position.y)
        {
            return astarNodeMgr.GetNode(current.x, current.y);
        } 
        if (xDirect != 0 && yDirect != 0)
        {
            //对角
            if (JudgeDiagonalNeighbor(current, new Vector2Int(xDirect, yDirect)))
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }

            //水平跳点搜索
            if(Jump(current + new Vector2Int(xDirect, 0), xDirect, 0, end)!=null)
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }
            //垂直跳点搜索
            if (Jump(current + new Vector2Int(0, yDirect), 0, yDirect, end) != null)
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }
        }
        else
        {
            //水平或垂直
            if (JudgeLinelNeighbor(current, new Vector2Int(xDirect, yDirect)))
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }
        }
        return Jump(current + new Vector2Int(xDirect, yDirect), xDirect, yDirect, end);
    }

    /// <summary>
    /// 找水平/垂直方向的强迫邻居
    /// </summary>
    /// <param name="current"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    List<Vector2Int> GetLineForceNeighbor(Vector2Int current,Vector2Int direction)
    {
        List<Vector2Int> lineForceNeighborArray = new List<Vector2Int>();
        Vector2Int dPosReverse = new Vector2Int(direction.y, direction.x);
        Vector2Int neighbor1 = current + dPosReverse;
        Vector2Int jump1 = neighbor1 + direction;
        Vector2Int neighbor2 = current - dPosReverse;
        Vector2Int jump2 = neighbor2 + direction;
        //再判断强迫邻居
        if (!astarNodeMgr.IsWalkable(neighbor1.x, neighbor1.y) && astarNodeMgr.IsWalkable(jump1.x, jump1.y))
        {
            lineForceNeighborArray.Add(jump1);
        }

        if (!astarNodeMgr.IsWalkable(neighbor2.x, neighbor2.y) && astarNodeMgr.IsWalkable(jump2.x, jump2.y))
        {
            lineForceNeighborArray.Add(jump2);
        }
        return lineForceNeighborArray;
    }

    /// <summary>
    /// 找对角线的强迫邻居
    /// </summary>
    /// <param name="current"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    List<Vector2Int> GetDiagonalNeighbor(Vector2Int current, Vector2Int direction)
    {
        List<Vector2Int> diagonalForceNeighborArray = new List<Vector2Int>();
        Vector2Int neighbor1 = new Vector2Int(current.x - direction.x, current.y);
        Vector2Int jump1 = neighbor1 + new Vector2Int(0, direction.y);
        Vector2Int neighbor2 = new Vector2Int(current.x, current.y - direction.y);
        Vector2Int jump2 = neighbor2 + new Vector2Int(direction.x, 0);
        if (!astarNodeMgr.IsWalkable(neighbor1.x, neighbor1.y) && astarNodeMgr.IsWalkable(jump1.x, jump1.y))
        {
            diagonalForceNeighborArray.Add(jump1);
        }

        if (!astarNodeMgr.IsWalkable(neighbor2.x, neighbor2.y) && astarNodeMgr.IsWalkable(jump2.x, jump2.y))
        {
            diagonalForceNeighborArray.Add(jump2);
        }
        return diagonalForceNeighborArray;
    }

    /// <summary>
    /// 判断对角线的强迫邻居存在否
    /// </summary>
    /// <param name="current"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    bool JudgeDiagonalNeighbor(Vector2Int current, Vector2Int direction)
    {
        Vector2Int neighbor1 = new Vector2Int(current.x - direction.x, current.y);
        Vector2Int jump1 = neighbor1 + new Vector2Int(0, direction.y);
        Vector2Int neighbor2 = new Vector2Int(current.x, current.y - direction.y);
        Vector2Int jump2 = neighbor2 + new Vector2Int(direction.x, 0);
        if (!astarNodeMgr.IsWalkable(neighbor1.x, neighbor1.y) && astarNodeMgr.IsWalkable(jump1.x, jump1.y))
        {
            return true;
        }

        if (!astarNodeMgr.IsWalkable(neighbor2.x, neighbor2.y) && astarNodeMgr.IsWalkable(jump2.x, jump2.y))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断水平/垂直的强迫邻居存在否
    /// </summary>
    /// <param name="current"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    bool JudgeLinelNeighbor(Vector2Int current, Vector2Int direction)
    {
        Vector2Int dPosReverse = new Vector2Int(direction.y, direction.x);
        Vector2Int neighbor1 = current + dPosReverse;
        Vector2Int jump1 = neighbor1 + direction;
        Vector2Int neighbor2 = current - dPosReverse;
        Vector2Int jump2 = neighbor2 + direction;
        //再判断强迫邻居
        if (!astarNodeMgr.IsWalkable(neighbor1.x, neighbor1.y) && astarNodeMgr.IsWalkable(jump1.x, jump1.y))
        {
            return true;
        }

        if (!astarNodeMgr.IsWalkable(neighbor2.x, neighbor2.y) && astarNodeMgr.IsWalkable(jump2.x, jump2.y))
        {
            return true;
        }
        return false; 
    }

    /// <summary>
    /// 节点实际路径代价G值计算
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int CalcG(Node a, Node b)
    {

        int xDiff = Mathf.Abs(a.Position.x - b.Position.x);
        int yDiff = Mathf.Abs(a.Position.y - b.Position.y);
        if (xDiff == yDiff)
        {
            return xDiff * 14;
        }
        else
        {
            return Mathf.Min(xDiff, yDiff) * 14 + Mathf.Abs(xDiff - yDiff) * 10;
        }

    }

    /// <summary>
    /// 节点启发路径代价H值计算
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public int CalcH(Node a, Node b)
    {
        int step = Mathf.Abs(a.Position.x- b.Position.x) + Mathf.Abs(a.Position.y-b.Position.y);
        return step * 10;
    }

    /// <summary>
    /// 路径回溯
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
     List<Node> ReversePath(Node node)
    {
        List<Node> path = new List<Node>();
        Node current = node;
        while (current != null)
        {
            //if (!path.Contains(current))
            //{
            path.Add(current);
            current = current.Parent;
            //}

        }
        path.Reverse();
        //Debug.Log("PATH.count " + path.Count);
        return path;
    }
    #endregion
}
