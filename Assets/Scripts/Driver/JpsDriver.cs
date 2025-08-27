using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum ClickState { start, end, obs, normal }
public class JpsDriver: MonoBehaviour
{ 
    public JpsComponent JpsCompot;
    public static JpsComponent startCompot;
    public static JpsComponent endCompot;
    public int width;
    public int height;
    public Transform gridStartTrans;
    GridMgr GridMgr;
    public static ClickState clickState;
    public Vector2Int defalutStartPos;
    public Vector2Int defalutEndPos;
    public Button jpsBtn;
    public Button resetBtn;
    public JpsFindBrain jpsBrain;


    void Start()
    {
        clickState = ClickState.normal;
        GridMgr = new GridMgr(width, height, 35, JpsCompot, gridStartTrans);
        jpsBrain = new JpsFindBrain(GridMgr.nodeMgr);
        GridMgr.GenerateGrid(gridStartTrans);
        SetDefaultClickState();
        jpsBtn.onClick.AddListener(JpsBtnBind);
        resetBtn.onClick.AddListener(ResetBtnBind);
    }

    void SetDefaultClickState()
    {
        NodeComponent dStartNode=GridMgr.GetNodeComponent(defalutStartPos);
        if (dStartNode != null)
        {
            dStartNode.SetAsStart();
            startCompot =(JpsComponent) dStartNode;
        }

        NodeComponent dEndNode = GridMgr.GetNodeComponent(defalutEndPos);
        if (dEndNode != null)
        {
            dEndNode.SetAsEnd();
            endCompot = (JpsComponent)dEndNode;
        }

    }
    /// <summary>
    /// Jps寻路按钮
    /// </summary>
    public void JpsBtnBind()
    {
        if (JpsDriver.startCompot!=null && JpsDriver.endCompot!=null)
        { 
            List<Node> pathNodeArray = jpsBrain.FindPath(startCompot.node, endCompot.node);
            for (int i = 0; i < pathNodeArray.Count; i++)
            {
                Vector2Int pos = new Vector2Int(pathNodeArray[i].Position.x, pathNodeArray[i].Position.y);
                NodeComponent nodeCompot = GridMgr.GetNodeComponent(pos);

                if (nodeCompot.node.NodeType == NodeType.Normal)
                {
                    nodeCompot.SetAsPath();
                }
            }
        }
    }
    /// <summary>
    /// 重置按钮
    /// </summary>
    public void ResetBtnBind()
    {
        GridMgr.ResetAllNodes();
        SetDefaultClickState();
    }
}



