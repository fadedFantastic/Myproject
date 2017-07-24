using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;
using System;


//场景编辑窗口
public class SceneEditorWindow : EditorWindow
{
    public static SceneEditorWindow mInstance = null;

    //是否初始化
    public bool mInitialized = false;

    //保存场景信息
    public List<string> mSceneNameList = new List<string>();
    //当前选中场景
    public string mCurSceneName = string.Empty;
    //当前选中功能
    public string mFunMenu = string.Empty;    

    //场景操作触发
    private static SceneView.OnSceneFunc mOnSceneGUIFunc;

    // 背景颜色
    public Color mBackGroundColor = Color.gray;
    //编辑器操作方向
    public Vector3 mInputDir = Vector3.zero;
    //场景编辑器模拟测试角色
    public Player mSimulatePlayer = null;
    public bool mIsMove = false;
    private float mTotalIdleTime = 1.0f;

    //窗口成员
    public static SceneEditorWindow Instance()
    {
        if (null == mInstance)
        {
            mInstance = EditorWindow.GetWindow<SceneEditorWindow>("SceneEditorWindow");

            //场景GUI
            mOnSceneGUIFunc = new SceneView.OnSceneFunc(OnSceneGUI);
            SceneView.onSceneGUIDelegate += mOnSceneGUIFunc;
            //层次GUI
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }
        return mInstance;
    }

    //窗口销毁操作
    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= mOnSceneGUIFunc;
        EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;


        DynamicModelMgr.Instance.Remove(10000);

        UnitMgr.Instance.DestoryAll();


        TerrainBlockMgr.Instance.Release();
        mInstance = null;
        mSimulatePlayer = null;
        mCurSceneName = string.Empty;
        mFunMenu = string.Empty;
        TerrainBlockWindow.Instance.mCurrentTerrainBlockName = string.Empty;
    }

    void Start(){}

    //初始化
    public void Initlialize()
    {
       
        //获取场景信息
        mSceneNameList.Clear();
        //场景路径
        string sceneDir = Application.dataPath + @"/Scenes";
        //遍历文件夹,获取模型名称
        string[] sceneNames = new string[] { };
        sceneNames = Directory.GetFiles(sceneDir, "*.unity", SearchOption.TopDirectoryOnly);
    
        //保存场景名称
        foreach (var pathName in sceneNames)
        {
            string name = Path.GetFileNameWithoutExtension(pathName);
            mSceneNameList.Add(name);
        }
        //保存背景颜色
        mBackGroundColor = GUI.backgroundColor;

        //*****************************释放
        OnDestroy();


        //****************************初始化
        //角色配置表
        PlayerPropertyMgr.Instance.Load();
        //怪物配置表
        MonsterPropertyMgr.Instance.Load();

        CameraMgr.Instance.Init();     
        Debug.Log("------------------initialize");
        mInitialized = true;
    }

    //创建模拟测试角色
    public Player CreateSimulatePlayer()
    {
        if(mSimulatePlayer == null)
        {
            //mSimulatePlayer = UnitMgr.Instance.CreateUnit<Player>(1, 10000, "BN_danbing", "Player", new Vector3(133, 11, 13), Quaternion.identity) as Player;      

             mSimulatePlayer = UnitMgr.Instance.CreateUnit<Player>(3, 10000, "Dplayer001", UNIT_SIDE.Virtue, new Vector3(181, 76, 125 ), Quaternion.identity) as Player;

             //绑定相机
             FollowCamera camera = CameraMgr.Instance.GetCamera(CameraMode.Follow) as FollowCamera;
            camera.Bind(mSimulatePlayer.GetModelObj());
            CameraMgr.Instance.AtiveCamera(CameraMode.Follow);
        }
            
        return mSimulatePlayer;
    }


    private Vector2 pos1 = new Vector2(0, 0);
    void OnGUI()
    {
        if (!mInitialized)
            Initlialize();
        
        //场景选择
        GUILayout.BeginHorizontal();
        SceneUI();

        //功能选择UI
        FunctionUI();
        
        //详细属性UI 
        //PropertyUI();

        GUILayout.EndHorizontal();        
    }

    //按键
    static bool kPressOk = true;
    static bool kPressOk2 = false;
    //场景GUI
    static void OnSceneGUI(SceneView sceneView)
    {
        //输入处理
        SceneEditorWindow.Instance().OnInput();
                
        string menuName = SceneEditorWindow.Instance().mFunMenu;

        //地形阻挡编辑场景绘制
        if (menuName == "TerrainBlock")
            TerrainBlockWindow.Instance.OnSceneGUI();
        else if (menuName == "SceneCamera")
            SceneCameraWindow.Instance.OnSceneGUI();
        else if (menuName == "SceneStrop")
            SceneStropMgr.Instance.OnSceneGUI();
        else if (menuName == "SceneLift")
            SceneStropMgr.Instance.OnSceneGUI();
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        //按键处理
        if (UnityEngine.Event.current.type == EventType.keyDown)
        {            
            if(!kPressOk2)
            {
                //Debug.Log("--------------press" + UnityEngine.Event.current.keyCode.ToString());
                SceneEditorWindow.Instance().OnHierarchyInput(UnityEngine.Event.current.keyCode);                
                kPressOk2 = true;
            }
        }
        else if (UnityEngine.Event.current.type == EventType.KeyUp)
        {
            if(kPressOk2)
            {
                //Debug.Log("--------------up press" + UnityEngine.Event.current.keyCode.ToString());                
                kPressOk2 = false;
            }
        }
    }

    //层次窗口按键按下
    private void OnHierarchyInput(KeyCode key)
    {
        //当前为地形编辑功能
        if(mFunMenu== "TerrainBlock" && key == KeyCode.K)
        {
            TerrainBlockWindow.Instance.DeleteTerrainKeyPoint();
        }
    }


    float xDir = 0;
    float zDir = 0;

    //输入处理
    private void OnInput()
    {
     
        Event current = Event.current;

        //鼠标按下
        if (current.type == EventType.MouseDown)
        {
            if (UnityEngine.Event.current.button == 0)
            {
                //Debug.Log("-------------left mouse down");
                SceneOperationMgr.Instance.OnLeftMouseDown(UnityEngine.Event.current.mousePosition);
            }
            else if (UnityEngine.Event.current.button == 1)
            {
                //Debug.Log("-------------right mouse down");
                SceneOperationMgr.Instance.OnRightMouseDown(UnityEngine.Event.current.mousePosition);
            }
        }
        //鼠标放开
        else if (current.type == EventType.MouseUp)
        {
            if (UnityEngine.Event.current.button == 0)
            {
                //Debug.Log("-------------left mouse up");
                SceneOperationMgr.Instance.OnLeftMouseUp(UnityEngine.Event.current.mousePosition);

            }
            else if (UnityEngine.Event.current.button == 1)
            {
                //Debug.Log("-------------right mouse up");
                SceneOperationMgr.Instance.OnRightMouseUp(UnityEngine.Event.current.mousePosition);

            }
        }
        //鼠标移动
        else if (current.type == EventType.MouseMove)
        {
            //Debug.Log("-------------mouse move");
            SceneOperationMgr.Instance.OnMouseMove(UnityEngine.Event.current.mousePosition);
        }

        //鼠标拖拽
        if (UnityEngine.Event.current.type == EventType.MouseDrag)
        {
            //左键拖动
            if (UnityEngine.Event.current.button == 0)
                SceneOperationMgr.Instance.OnLeftMouseDrag(UnityEngine.Event.current.mousePosition);
        }

        Vector3 u = Camera.main.transform.right;
        Vector3 v = Camera.main.transform.forward;
        u.y = 0;
        v.y = 0;
        u.Normalize();
        v.Normalize();



        //按键处理
        if (UnityEngine.Event.current.type == EventType.keyDown)
        {
            //一直按下
            SceneOperationMgr.Instance.OnKeyPress(UnityEngine.Event.current.keyCode);

            //WASD操作
            if (UnityEngine.Event.current.keyCode == KeyCode.W)
            {
                zDir = 1;
            }
            else if (UnityEngine.Event.current.keyCode == KeyCode.A)
            {
                xDir = -1;
            }
            else if (UnityEngine.Event.current.keyCode == KeyCode.S)
            {
                zDir = -1;
            }
            else if (UnityEngine.Event.current.keyCode == KeyCode.D)
            {
                xDir = 1;
            }

            mInputDir = u * xDir + v * zDir;
            mInputDir.Normalize();

            //Debug.Log("----------------iput" + mInputDir.ToString());
           
            //跳跃处理
            if (UnityEngine.Event.current.keyCode == KeyCode.Space && mSimulatePlayer != null)
            {
                mSimulatePlayer.OnMessageJump();
            }

            if (kPressOk)
            {
                OnHierarchyInput(UnityEngine.Event.current.keyCode);

                //Debug.Log("--------------press k" + UnityEngine.Event.current.keyCode.ToString());
                SceneOperationMgr.Instance.OnKeyDown(UnityEngine.Event.current.keyCode);
                kPressOk = false;
            }
        }
        else if (UnityEngine.Event.current.type == EventType.keyUp)
        {
            //WASD操作
            if (UnityEngine.Event.current.keyCode == KeyCode.W)
            {
                zDir = 0;
            }
            else if (UnityEngine.Event.current.keyCode == KeyCode.A)
            {
                xDir = 0;
            }
            else if (UnityEngine.Event.current.keyCode == KeyCode.S)
            {
                zDir = 0;
            }
            else if (UnityEngine.Event.current.keyCode == KeyCode.D)
            {
                xDir = 0;
            }
            mInputDir = u * xDir + v * zDir;
            mInputDir.Normalize();

            //Debug.Log("--------------un press k" + UnityEngine.Event.current.keyCode.ToString());
            if (!kPressOk)
            {
                kPressOk = true;
            }
        }
    }

    //模拟玩家操作输入
    public void OnSimulatePlayerInput(float deltaTime)
    {
        //是否开启输入
        if (!InputMgr.Instance.mEnable)
        {          
            return;
        }


        if (mSimulatePlayer == null)
            return;     

        if (mInputDir != Vector3.zero)
        {            
            mIsMove = true;         
            Vector3 move = mInputDir * mSimulatePlayer.mMoveSpeed * deltaTime;
            mSimulatePlayer.mInputMove = move;
            mSimulatePlayer.mInputDir = mInputDir;
            mSimulatePlayer.OnMessage_Input_Move();
        }
        else
        {
            //累计暂停时间超过0.02s,才进入站立状态
            mTotalIdleTime += deltaTime;
            if (mTotalIdleTime >= 0.1f)
            {
                if (mIsMove)
                {
                    //mSimulatePlayer.mInputMove = move;
                    mSimulatePlayer.mInputDir = mInputDir;
                    mSimulatePlayer.OnMessage_Input_Stand();
                    mIsMove = false;
                }
            }
        }
        //朝向        
        mSimulatePlayer.OnMessage_Input_Dir(mInputDir);
    }


    //场景GUI
    void SceneUI()
    {
        pos1 = GUILayout.BeginScrollView(pos1, false, true, GUILayout.Width(150), GUILayout.Height(800));
        GUILayout.BeginVertical();
        foreach (string sceneName in mSceneNameList)
        {
            //选中
            if (mCurSceneName != string.Empty && mCurSceneName == sceneName)
            {
                GUI.backgroundColor = Color.green;
                //切换场景
                if (GUILayout.Button(sceneName, GUILayout.Width(120)))
                {
                    mCurSceneName = sceneName;
                    ChangeScene(sceneName);
                }
                GUI.backgroundColor = mBackGroundColor;
            }
            else
            {
                //切换场景
                if (GUILayout.Button(sceneName, GUILayout.Width(120)))
                {
                    mCurSceneName = sceneName;
                    ChangeScene(sceneName);
                }
            }
        }
    
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

 
    //功能UI
    void FunctionUI()
    {
        GUILayout.BeginVertical();

        //编辑地形阻挡
        GUILayout.Button("场景功能列表");
        
        //地形阻挡
        if(GUILayout.Button("地形阻挡", GUILayout.Width(100)))
        {
            mFunMenu = "TerrainBlock";
            SceneEditorPropertyWindow.Instance().Repaint();
        }
        //场景相机
        else if (GUILayout.Button("场景相机", GUILayout.Width(100)))
        {
            mFunMenu = "SceneCamera";
            SceneEditorPropertyWindow.Instance().Repaint();
        }
        //场景滑索
        else if(GUILayout.Button("场景滑索", GUILayout.Width(100)))
        {
            mFunMenu = "SceneStrop";
            SceneEditorPropertyWindow.Instance().Repaint();
        }
        //场景升降机
        else if (GUILayout.Button("场景升降机", GUILayout.Width(100)))
        {
            mFunMenu = "SceneLift";
            SceneEditorPropertyWindow.Instance().Repaint();
        }
        //模型触发器
        else if (GUILayout.Button("模型触发器", GUILayout.Width(100)))
        {
            mFunMenu = "DynaObj";
            SceneEditorPropertyWindow.Instance().Repaint();
        }

        GUILayout.EndVertical();
    }

    //属性UI 
    public void PropertyUI()
    {
        if(mFunMenu == "TerrainBlock")
        {
            TerrainBlockWindow.Instance.TerrainBlockPropertyUI();
        }
        else if(mFunMenu == "SceneCamera")
        {           
            SceneCameraWindow.Instance.SceneCameraPropertyUI();
        }
        else if(mFunMenu == "SceneStrop")
        {
            SceneStropWindow.Instance.SceneStropPropertyUI();
        }
        else if(mFunMenu == "SceneLift")
        {
            SceneLiftWindow.Instance.SceneLiftPropertyUI();
        }
        else if(mFunMenu == "DynaObj")
        {
            DynaObjWindow.Instance.DynaObjPropertyUI();
        }
    }

    
    //切换场景
    public void ChangeScene(string name)
    {
        Release();      
        string sceneName = string.Format(@"Assets\Scenes\{0}.unity", name);
        UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(sceneName);

        //重新初始地形阻挡
        TerrainBlockMgr.Instance.Init();
        //重新初始相机平面控制
        CameraPlaneCtrlMgr.Instance.Init();

        //重新初始场景滑索
        SceneStropMgr.Instance.Init();
        //重新初始化场景升降机
        SceneLiftMgr.Instance.Init();
        //重新初始化模型触发器 
        DynaObjMgr.Instance.Init();

        //输入开启
        InputMgr.Instance.mEnable = true;
        //创建模型节点
        GameObject gameModelNode = new GameObject("[dynamic_model]");
    }

    //释放场景资源
    public void Release()
    {
    }

    //更新
    public void Updated(float deltaTime)
    {

    }
}
