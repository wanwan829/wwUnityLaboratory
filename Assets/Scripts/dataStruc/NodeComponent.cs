using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Start, End, Normal,Obs,Path
}
public class NodeComponent: MonoBehaviour
{
    public Node node = new Node();
    public void Awake()
    {
        node.NodeType = NodeType.Normal;
    }

    public virtual void Clear()
    {

    }

    public virtual void SetAsObstacle() {
        node.IsWalkable = false;
    }
    public virtual void SetAsPath() { }
    public virtual void SetAsDefault() { }
    public virtual void SetAsStart() { }
    public virtual void SetAsEnd() { }

}
