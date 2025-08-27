using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JpsFindBrain : IFindPathAlgor
{

    #region ��������
    private int MapWidth;
    private int MapHeight;
    private MiniHeap<Node> openSet;
    private HashSet<Node> closeSet;
    private AstarNodeMgr astarNodeMgr;
    public List<Node> jumpPointArray = new List<Node>();
    public List<Node> jumpPointArray1 = new List<Node>();
    #endregion
    #region ���캯��
    public JpsFindBrain(AstarNodeMgr astarNodeMgr)
    {
        this.astarNodeMgr = astarNodeMgr;
        openSet = new MiniHeap<Node>();
        closeSet = new HashSet<Node>();
    }
    #endregion
    #region ��������
    /// <summary>
    /// Ѱ·�ӿ�
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<Node> FindPath(Node start, Node end)
    {
        //����
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
        Debug.Log("û���ҵ�·��");
        return null;
    }

    /// <summary>
    /// Ѱ·���ڵ㽨��
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    Node FindRootNode(Node start,Node end)
    {
        //��������G.Hֵ�������뿪���б�
        start.G = 0;
        start.H = CalcH(start, end);
        openSet.Push(start);
        while (openSet.Count != 0)  
        {
            //�����б�ó���Сֵ
            Node cur = openSet.Pop();
            //�ر��б���м�¼
            closeSet.Add(cur);
            //��ǰ�ڵ����Ŀ��㣬���ص�ǰ�ڵ㡣
            if (cur == end)
            {
                return cur;
            }
            //��ȡ��ǰ�ڵ��ھ��б���̽�������б�
            List<Vector2Int> Neighbors = GetNeighbors(cur);
            //�����ھ��б�
            for (int i = 0; i < Neighbors.Count; i++)
            {   
                Vector2Int dir = Neighbors[i] - cur.Position;
                //Ѱ�ҵ�ǰ��������
                var jumpPoint = Jump(Neighbors[i], dir.x, dir.y, end);
                //����ҵ�����
                if (jumpPoint != null && !closeSet.Contains(jumpPoint))
                {   
                    //���������G,H,F���Լ�Parent
                    jumpPoint.G = cur.G + CalcG(cur, jumpPoint);
                    jumpPoint.H = CalcH(jumpPoint, end);
                    jumpPoint.F = jumpPoint.G + jumpPoint.H;
                    jumpPoint.Parent = cur;
                    //��������뿪���б�
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
    /// ��ȡ�ھӵ�
    /// </summary>
    /// <param name="node"></param>
    List<Vector2Int> GetNeighbors(Node node)
    {
        List<Vector2Int> directions = new List<Vector2Int>();
        //�ڵ�ĸ��ڵ�Ϊ�գ�����һ���ڵ�
        if(node.Parent == null)
        {
            //̽���˸�����
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
        //�жϵ�ǰ�ڵ�͸��ڵ�ķ����ϵ ����/�Խ�
        int finalX = Mathf.Clamp(node.Position.x - node.Parent.Position.x,-1,1);
        int finalY = Mathf.Clamp(node.Position.y - node.Parent.Position.y, -1, 1);
        Vector2Int dPos = new Vector2Int(finalX,finalY);
        //������ͶԽ���ֿ�����
        if (dPos.x == 0 || dPos.y == 0)
        {
      
            //�ж���ǰ���Ƿ����
            bool forward = astarNodeMgr.IsWalkable(node.Position.x + dPos.x, node.Position.y + dPos.y);
            if (forward)
            {
                directions.Add(new Vector2Int(node.Position.x + dPos.x, node.Position.y + dPos.y));
            }
            //�ж��Ƿ����ǿ���ھӷ���Ĵ������뷽��
            List<Vector2Int> forceDirs = GetLineForceNeighbor(node.Position, dPos);
            if (forceDirs.Count > 0)
            {   
                //����ǿ���ھӣ��������
                for (int i = 0; i < forceDirs.Count; i++)
                {
                    directions.Add(forceDirs[i]);

                }
            }
        }
        else
        {
            //���ж϶Խ�������������Ƿ����
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
            //�ж϶Խ����ǿ���ھ��Ƿ����
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
    /// Ѱ������
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
            //�Խ�
            if (JudgeDiagonalNeighbor(current, new Vector2Int(xDirect, yDirect)))
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }

            //ˮƽ��������
            if(Jump(current + new Vector2Int(xDirect, 0), xDirect, 0, end)!=null)
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }
            //��ֱ��������
            if (Jump(current + new Vector2Int(0, yDirect), 0, yDirect, end) != null)
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }
        }
        else
        {
            //ˮƽ��ֱ
            if (JudgeLinelNeighbor(current, new Vector2Int(xDirect, yDirect)))
            {
                return astarNodeMgr.GetNode(current.x, current.y);
            }
        }
        return Jump(current + new Vector2Int(xDirect, yDirect), xDirect, yDirect, end);
    }

    /// <summary>
    /// ��ˮƽ/��ֱ�����ǿ���ھ�
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
        //���ж�ǿ���ھ�
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
    /// �ҶԽ��ߵ�ǿ���ھ�
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
    /// �ж϶Խ��ߵ�ǿ���ھӴ��ڷ�
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
    /// �ж�ˮƽ/��ֱ��ǿ���ھӴ��ڷ�
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
        //���ж�ǿ���ھ�
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
    /// �ڵ�ʵ��·������Gֵ����
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
    /// �ڵ�����·������Hֵ����
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
    /// ·������
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
