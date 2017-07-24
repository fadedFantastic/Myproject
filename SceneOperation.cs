using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.Collections;

//[yangaizhou]场景编辑器操作
public abstract class SceneOperation
{
    //左键是否按下
    public bool mIsLeftButtonDown = false;
    //右键是否按下
    public bool mIsRightButtonDown = false;


    //左键按下位置
    public Vector2 mLeftButtonDownPos;

    //右键按下位置
    public Vector2 mRightButtonDownPos;

    //鼠标当前移动位置 
    public Vector2 mMouseMovePos;

    //操作包含的信息
    public object mParam = null;

    //操作名称
    public string mName = "SceneOperation";

    public SceneOperation()
    {

    }

    //鼠标左键按下
    public virtual void OnLeftMouseDown(Vector2 pt)
    {
        mIsLeftButtonDown = true;
        mLeftButtonDownPos = pt;
        mMouseMovePos = pt;
    }

    //鼠标左键放开
    public virtual void OnLeftMouseUp(Vector2 pt)
    {
     
        mIsLeftButtonDown = false;
    }

    //鼠标右键按下
    public virtual void OnRightMouseDown(Vector2 pt)
    {
        mIsRightButtonDown = true;
        mRightButtonDownPos = pt;
        //mMouseMovePos = pt;
    }


    //鼠标右键放开 
    public virtual void OnRightMouseUp(Vector2 pt)
    {
        mIsRightButtonDown = false;
    }

    //鼠标移动
    public virtual void OnMouseMove(Vector2 pt)
    {
        mMouseMovePos = pt;
    }

    //鼠标拖动
    public virtual void OnLeftMouseDrag(Vector2 pt)
    {
        mMouseMovePos = pt;
        mIsLeftButtonDown = true;
    }

    //鼠标中键滚动 
    public virtual void OnMouseWheel(float delta)
    {
    }

    //按键操作
    public virtual void OnKeyDown(KeyCode key)
    {

    }

    //一直按下操作
    public virtual void OnKeyPress(KeyCode key)
    {
    }

    //操作中的绘制操作
    public virtual void Draw()
    {

    }
}



//编辑地形碰撞操作
public class CreateTerrainBlockHandle : SceneOperation
{
    //当前Block名称
    public string mBlockName = string.Empty;
    //操作是否结束
    public bool mIsFinished = true;
    
    public CreateTerrainBlockHandle()
    {
    }

    //鼠标左键按下
    public override void OnLeftMouseDown(Vector2 pt)
    {
        base.OnLeftMouseDown(pt);
                
        //场景中没有,创建Block以及第一个关键点
        if (mBlockName == string.Empty)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(pt);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, 10000, -1))
            {
                TerrainBlock terrainBlock = TerrainBlockMgr.Instance.CreateTerrainBlock();         
                //添加
                //terrainBlock.AddKeyObj(hitInfo.point);

                TerrainBlockMgr.Instance.SetHandlePosition(hitInfo.point);
                TerrainBlockMgr.Instance.ShowHandle(true);

                //设置当前块
                TerrainBlockWindow.Instance.mCurrentTerrainBlockName = terrainBlock.mName;

                mBlockName = terrainBlock.mName;
                mIsFinished = false;
            }
        }            
    }

    public override void OnKeyDown(KeyCode key)
    {
        //创建新的阻挡
        if(key == KeyCode.C)
        {
            AddHandleBlock();
        }
    }

    //鼠标左键放开 
    public override void OnLeftMouseUp(Vector2 pt)
    {        
        base.OnLeftMouseUp(pt);        
    }

    //鼠标移动
    public override void OnLeftMouseDrag(Vector2 pt)
    {
        base.OnLeftMouseDrag(pt);
    }

    //鼠标右键按下结束操作
    public override void OnRightMouseDown(Vector2 pt)
    {      
        //右键弹出完成窗口
        TerrainBlockFinishWindow.Instance().Show();

        base.OnRightMouseDown(pt);      
    }

    //根据Handle位置添加地形Block
    public void AddHandleBlock()
    {
        //选中Handle阻挡
        if (Selection.activeGameObject == TerrainBlockMgr.Instance.mHandleBlockObj)
        {
            TerrainBlock terrainBlock = TerrainBlockMgr.Instance.GetTerrainBlock(mBlockName);
            if (terrainBlock == null)
                return;

            //添加新的阻挡
            terrainBlock.AddKeyObj(Selection.activeGameObject.transform.position);
        }
    }
    
    //根据位置添加Block
    public void AddBlock(Vector3 pos)
    {
        TerrainBlock terrainBlock = TerrainBlockMgr.Instance.GetTerrainBlock(mBlockName);
        if (terrainBlock == null)
            return;

        //添加新的阻挡
        terrainBlock.AddKeyObj(pos);
    }

    //结束地形阻挡体编辑, 传入线段是否封闭
    public void FinishOperation(bool mIsClose)
    {
       
        TerrainBlock terrainBlock = TerrainBlockMgr.Instance.GetTerrainBlock(mBlockName);
        if (terrainBlock == null)
            return;

        //封闭结束操作
        if (mIsClose)
        {
            //添加操作点
            AddHandleBlock();

            //封闭添加起点
            if (terrainBlock.mKeyObjList.Count != 0)
            {
                AddBlock(terrainBlock.mKeyObjList[0].transform.position);
            }
        }   
        else
        {
            AddHandleBlock();
        }

        //结束编辑状态
        terrainBlock.mIsEdit = false;
        mIsFinished = true;
        TerrainBlockMgr.Instance.ShowHandle(false);
        mBlockName = string.Empty;
        SceneOperationMgr.Instance.SetCurrentOperation("NULL", null);
    }
}



//编辑相机平面操作
public class CreateCameraPlaneCtrlHandle : SceneOperation
{
    public CreateCameraPlaneCtrlHandle()
    {
    }

    //鼠标左键按下
    public override void OnLeftMouseDown(Vector2 pt)
    {
        base.OnLeftMouseDown(pt);

        //节点列表为空,直接创建
        if(CameraPlaneCtrlMgr.Instance.mCtrlMap.Count == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(pt);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, 10000, -1))
            {
                CameraPlaneCtrl ctrl = CameraPlaneCtrlMgr.Instance.CreateCameraPlaneCtrl(CameraPlaneCtrlMgr.mGuid,hitInfo.point,Quaternion.identity, new Vector3(12,5,5),new CameraPlaneCtrl.CameraInfo());
                CameraPlaneCtrlMgr.mGuid++;
                //设置当前Ctrl
                CameraPlaneCtrlMgr.Instance.mCurCtrName = ctrl.mName;
                SceneEditorPropertyWindow.Instance().Repaint();
            }
        }
        else
        {            
            //获取当前选中Ctrl
            string curCtrlName = CameraPlaneCtrlMgr.Instance.mCurCtrName;
            if (curCtrlName == string.Empty)
                return;

            CameraPlaneCtrl ctrl = CameraPlaneCtrlMgr.Instance.GetCameraPlaneCtrl(int.Parse(curCtrlName));

            //存在
            if (ctrl != null)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(pt);
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(ray, out hitInfo, 10000, -1))
                {
                    CameraPlaneCtrl childCtrl = CameraPlaneCtrlMgr.Instance.CreateCameraPlaneCtrl(CameraPlaneCtrlMgr.mGuid,hitInfo.point, Quaternion.identity, new Vector3(12, 5, 5), new CameraPlaneCtrl.CameraInfo());
                    CameraPlaneCtrlMgr.mGuid++;

                    //设置当前Ctrl
                    CameraPlaneCtrlMgr.Instance.mCurCtrName = ctrl.mName;

                    //添加子Ctrl
                    ctrl.AddChild(childCtrl);
                    SceneEditorPropertyWindow.Instance().Repaint();
                }
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "需要选中平面切换相机!", "OK");
            }
        }

        SceneOperationMgr.Instance.SetCurrentOperation("NULL", null);
    }

    //鼠标左键放开 
    public override void OnLeftMouseUp(Vector2 pt)
    {
        base.OnLeftMouseUp(pt);
        
    }

    //鼠标移动
    public override void OnLeftMouseDrag(Vector2 pt)
    {
        base.OnLeftMouseDrag(pt);
    }

    //鼠标右键按下结束操作
    public override void OnRightMouseDown(Vector2 pt)
    {      
        base.OnRightMouseDown(pt);
    }
}

//创建场景滑索
public class CreateSceneStropHandle : SceneOperation
{
    //当前滑索ID    
    public int mID = -1;
    //操作是否结束
    public bool mIsFinished = true;

    public CreateSceneStropHandle()
    {
    }

    //鼠标左键按下
    public override void OnLeftMouseDown(Vector2 pt)
    {
        base.OnLeftMouseDown(pt);

        //场景中没有,创建滑索以及第一个关键点
        if (mID == -1)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(pt);
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(ray, out hitInfo, 10000, -1))
            {
                SceneStrop strop = SceneStropMgr.Instance.CreateSceneStrop();
                //设置触发器位置
                strop.mTriggerObj.transform.localPosition = hitInfo.point;

                SceneStropMgr.Instance.SetHandlePosition(hitInfo.point);
                SceneStropMgr.Instance.ShowHandle(true);
          
                //设置当前块
                SceneStropMgr.Instance.mCurStropName = strop.mName;

                mID = strop.mID;
                mIsFinished = false;
                strop.mIsEdit = true;
            }
        }
    }

    public override void OnKeyDown(KeyCode key)
    {
        //创建新的阻挡
        if (key == KeyCode.C)
        {
            AddHandleStrop();
        }
    }

    //鼠标左键放开 
    public override void OnLeftMouseUp(Vector2 pt)
    {
        base.OnLeftMouseUp(pt);
    }

    //鼠标移动
    public override void OnLeftMouseDrag(Vector2 pt)
    {
        base.OnLeftMouseDrag(pt);
    }

    //鼠标右键按下结束操作
    public override void OnRightMouseDown(Vector2 pt)
    {
        //右键弹出完成窗口
        SceneStropFinishWindow.Instance().Show();

        base.OnRightMouseDown(pt);
    }

    //根据Handle位置添加Strop位置
    public void AddHandleStrop()
    {
        //选中Handle阻挡
        if (Selection.activeGameObject == SceneStropMgr.Instance.mHandleObj)
        {
            SceneStrop strop = SceneStropMgr.Instance.GetSceneStrop(mID);
            if (strop == null)
                return;

            //添加新的阻挡
            strop.AddKeyObj(Selection.activeGameObject.transform.position);
        }
    }

    //根据位置添加Block
    public void AddBlock(Vector3 pos)
    {
        SceneStrop strop = SceneStropMgr.Instance.GetSceneStrop(mID);
        if (strop == null)
            return;

        //添加新的阻挡
        strop.AddKeyObj(pos);
    }

    //结束地形阻挡体编辑, 传入线段是否封闭
    public void FinishOperation()
    {
        SceneStrop sceneStrop = SceneStropMgr.Instance.GetSceneStrop(mID);
        if (sceneStrop == null)
            return;
 
        AddHandleStrop();
 
        //结束编辑状态
        sceneStrop.mIsEdit = false;
        mIsFinished = true;
        SceneStropMgr.Instance.ShowHandle(false);
        mID = -1;
        SceneOperationMgr.Instance.SetCurrentOperation("NULL", null);
    }
}

//创建场景升降机 
public class CreateSceneLiftHandle : SceneOperation
{    
    public CreateSceneLiftHandle()
    {
    }

    //鼠标左键按下
    public override void OnLeftMouseDown(Vector2 pt)
    {
        base.OnLeftMouseDown(pt);

        Ray ray = HandleUtility.GUIPointToWorldRay(pt);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, 10000, -1))
        {
            string resName = this.mParam as string;
            if(resName != string.Empty)
            {
                SceneLift lift = SceneLiftMgr.Instance.CreateSceneLift(resName,hitInfo.point,10,3);
            }
        }

        SceneOperationMgr.Instance.SetCurrentOperation("NULL", null);
    }
}




//创建模型触发器 
public class CreateDynaObjHandle : SceneOperation
{
    public CreateDynaObjHandle()
    {
    }

    //鼠标左键按下
    public override void OnLeftMouseDown(Vector2 pt)
    {
        base.OnLeftMouseDown(pt);

        Ray ray = HandleUtility.GUIPointToWorldRay(pt);
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, 10000, -1))
        {
            //[yangaizhou] 临时处理，名称需要单独处理
            string param  = mParam as string;
            string[] prams = param.Split('|');

            string modelName = prams[0];
            DynaObjType type = (DynaObjType)Enum.Parse(typeof(DynaObjType), prams[1]);

            DynaObj trigger = DynaObjMgr.Instance.CreateDynaObj(0, modelName, type, hitInfo.point, Quaternion.identity);
        }
        SceneOperationMgr.Instance.SetCurrentOperation("NULL", null);

        
    }    
}



