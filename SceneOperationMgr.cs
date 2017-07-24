using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

//场景编辑操作基类
public class SceneOperationMgr : Singleton<SceneOperationMgr>
{
    //左键是否按下
    public bool mIsLeftButtonDown = false;
    //右键是否按下
    public bool mIsRightButtonDown = false;

    //操作代理
    private delegate SceneOperation OperationDelegate();

    //注册的操作
    private Dictionary<string, OperationDelegate> mRegisterOperationHandleList = new Dictionary<string, OperationDelegate>();

    //保存的操作列表
    private List<SceneOperation> mOperationList = new List<SceneOperation>();

    public static T CreateNew<T>() where T : new()
    {
        //return new T();
        return new T();
    }

    //注册所有操作
    public SceneOperationMgr()
    {
        //创建地形阻挡
        RegisterOperation("CreateTerrainBlockHandle", CreateNew<CreateTerrainBlockHandle>);
        //创建相机平面切换控制
        RegisterOperation("CreateCameraPlaneCtrlHandle", CreateNew<CreateCameraPlaneCtrlHandle>);
        //创建场景滑索
        RegisterOperation("CreateSceneStropHandle", CreateNew<CreateSceneStropHandle>);
        //创建场景升降机
        RegisterOperation("CreateSceneLiftHandle", CreateNew<CreateSceneLiftHandle>);
        //创建模型触发器
        RegisterOperation("CreateDynaObjHandle", CreateNew<CreateDynaObjHandle>);

        
        //RegisterOperation("MoveSkillTimeBarHandle", CreateNew<MoveSkillTimeBarHandle>);
        //RegisterOperation("MoveSkillEventHandle", CreateNew<MoveSkillEventHandle>);
        //RegisterOperation("MoveSkillRegionHandle", CreateNew<MoveSkillRegionHandle>);
    }

    //注册操作函数
    void RegisterOperation(string name, OperationDelegate operation)
    {
        mRegisterOperationHandleList.Add(name, operation);
    }

    //是否为空 
    bool IsEmpty()
    {
        return mRegisterOperationHandleList.Count == 0;
    }

    //设置当前操作
    public bool SetCurrentOperation(string type, object param)
    {
        //根据类型创建技能操作
        SceneOperation operation = null;
        if (mRegisterOperationHandleList.ContainsKey(type))
        {
            operation = mRegisterOperationHandleList[type]();
        }

        if (operation != null)
        {
            operation.mParam = param;
            mOperationList.Add(operation);
            return true;
        }
        else
        {
            mOperationList.Add(null);
            return false;
        }
    }

    //获取当前操作
    public SceneOperation GetCurrentOperation()
    {
        //int size = 
        int size = mOperationList.Count;
        if (size == 0)
            return null;

        SceneOperation operation = mOperationList[size - 1];

        if (operation != null)
        {
            return operation;
        }
        else
            return null;
    }

    public void OnLeftMouseDown(Vector2 pt)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnLeftMouseDown(pt);
        }
    }

    public void OnLeftMouseUp(Vector2 pt)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnLeftMouseUp(pt);
        }
    }

    public void OnRightMouseDown(Vector2 pt)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnRightMouseDown(pt);
        }
    }

    public void OnRightMouseUp(Vector2 pt)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnRightMouseUp(pt);
        }
    }

    public void OnMouseMove(Vector2 pt)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnMouseMove(pt);
        }
    }

    public void OnLeftMouseDrag(Vector2 pt)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnLeftMouseDrag(pt);
        }
    }

    public void OnMouseWheel(float delta)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnMouseWheel(delta);
        }
    }


    public void OnKeyDown(KeyCode key)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnKeyDown(key);
        }
    }

    public void OnKeyPress(KeyCode key)
    {
        SceneOperation operation = GetCurrentOperation();
        if (operation != null)
        {
            operation.OnKeyPress(key);
        }
    }

}
