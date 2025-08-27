using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class JpsComponent: NodeComponent
{
    SelfButton GridBtn;
    Image image;

    // Start is called before the first frame update
    void Awake()
    { 
        GridBtn = this.gameObject.transform?.GetComponent<SelfButton>();
        GridBtn.onClick.AddListener(GridBtnBind);
        GridBtn.onSelfBtnDown.AddListener(OnSelfBtnDownAction);
        GridBtn.onSelfBtnEnter.AddListener(OnSelfBtnEnter);
        GridBtn.onSelfBtnUp.AddListener(OnSelfBtnUpAction);
        image = this.gameObject.transform?.GetComponent<Image>();
    }


    public override void SetAsStart()
    {
        base.SetAsStart();
        image.color = Color.green;
        node.NodeType = NodeType.Start;
    }

    public override void SetAsEnd()
    {
        base.SetAsEnd();
        image.color = Color.blue;
        node.NodeType = NodeType.End;
    }

    public override void SetAsDefault()
    {
        base.SetAsDefault();
        image.color = Color.white;
        node.NodeType = NodeType.Normal;
    }

    public override void SetAsObstacle()
    {
        base.SetAsObstacle();
        image.color = Color.gray;
        node.NodeType = NodeType.Obs;
    }

    public override void SetAsPath()
    {
        base.SetAsPath();
        image.color = Color.cyan;
    }

    public void SetJumpPoint()
    {
        image.color = Color.red;
    }
    void OnSelfBtnDownAction()
    {
        if (node.NodeType == NodeType.Start)
        {
            JpsDriver.clickState = ClickState.start;
        }
        if (node.NodeType == NodeType.End)
        {
            JpsDriver.clickState = ClickState.end;
        }

        if (node.NodeType == NodeType.Normal)
        {
            JpsDriver.clickState = ClickState.obs;
        }

    }
    void OnSelfBtnEnter()
    {
        if (JpsDriver.clickState == ClickState.start)
        {
            JpsDriver.startCompot = this;
        }

        if (JpsDriver.clickState == ClickState.end)
        {
            JpsDriver.endCompot = this;
        }

        if (JpsDriver.clickState == ClickState.obs&&(node.NodeType!=NodeType.Start&&node.NodeType!=NodeType.End))
        {
            SetAsObstacle();
        }
    }
    void OnSelfBtnUpAction()
    {
        if (JpsDriver.clickState == ClickState.start&&node.NodeType==NodeType.Start&&JpsDriver.startCompot!=this)
        {
            JpsDriver.startCompot.SetAsStart() ;
            SetAsDefault();
            JpsDriver.clickState = ClickState.normal;
        }

        if (JpsDriver.clickState == ClickState.end && node.NodeType == NodeType.End && JpsDriver.endCompot != this)
        {
            JpsDriver.endCompot.SetAsEnd();
            SetAsDefault();
            JpsDriver.clickState = ClickState.normal;
        }

        if (JpsDriver.clickState == ClickState.obs)
        {
            JpsDriver.clickState = ClickState.normal;

        }

    }
    void GridBtnBind()
    {
        //Debug.Log($" GridBtn clickkk");
    }

}
