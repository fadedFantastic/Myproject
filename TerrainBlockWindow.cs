using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;
using System;

//地形碰撞编辑窗口编辑窗口
public class TerrainBlockWindow : Singleton<TerrainBlockWindow>
{
    //当前选中地形阻挡块
    public string mCurrentTerrainBlockName = string.Empty;


    //地形阻挡属性UI
    bool fold = false;
    Vector2 pos2 = Vector2.zero;
    //是否隐藏其他
    bool hideOthers = false;
    bool mHideOthers = false;
    //是否显示面片
    bool showMesh = true;
    bool mShowMesh = true;
 
    public void OnSceneGUI()
    {
        TerrainBlockMgr.Instance.OnSceneGUI();
    }

    public void TerrainBlockPropertyUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Button("地形阻挡功能");

        //加载地形阻挡
        if (GUILayout.Button("加载地形阻挡", GUILayout.Width(100)))
        {
            LoadTerrainBlock();
        }
        //创建地形阻挡
        else if (GUILayout.Button("创建地形阻挡", GUILayout.Width(100)))
        {
            CreateTerrainBlock();
        }
        //编辑地形阻挡 
        else if (GUILayout.Button("编辑地形阻挡", GUILayout.Width(100)))
        {
            EditTerrainBlock();
        }
        //删除地形阻挡
        else if (GUILayout.Button("删除地形阻挡", GUILayout.Width(100)))
        {
            DeleteTerrainBlock();
        }
        //拆分地形阻挡
        else if (GUILayout.Button("拆分地形阻挡", GUILayout.Width(100)))
        {
            SplitTerrainBlock();
        }

        GUILayout.BeginHorizontal();

        //显示地形阻挡列表信息
        pos2 = GUILayout.BeginScrollView(pos2, false, true, GUILayout.Width(150), GUILayout.Height(800));
        GUILayout.BeginVertical();
        foreach (var block in TerrainBlockMgr.Instance.mTerrainBlockMap)
        {
            string blockName = block.Value.mName;


            //选中状态绘制
            if (mCurrentTerrainBlockName != string.Empty && mCurrentTerrainBlockName == blockName)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button(blockName, GUILayout.Width(120)))
                {
                    mCurrentTerrainBlockName = blockName;
                    //设置选中状态
                    TerrainBlock terrainBlock = TerrainBlockMgr.Instance.GetTerrainBlock(blockName);
                    if (terrainBlock != null)
                    {
                        Selection.activeGameObject = terrainBlock.mTerrainBlockObj;
                    }
                    HideOtherBlocks(mHideOthers,mShowMesh);
                }
                GUI.backgroundColor =  SceneEditorWindow.Instance().mBackGroundColor;
            }
            else
            {
                if (GUILayout.Button(blockName, GUILayout.Width(120)))
                {
                    mCurrentTerrainBlockName = blockName;
                    //设置选中状态
                    TerrainBlock terrainBlock = TerrainBlockMgr.Instance.GetTerrainBlock(blockName);
                    if (terrainBlock != null)
                    {
                        Selection.activeGameObject = terrainBlock.mTerrainBlockObj;
                    }
                    HideOtherBlocks(mHideOthers, mShowMesh);
                }
            }
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();


        //************属性信息生成
        GUILayout.BeginVertical();

        //隐藏其他
        GUILayout.BeginHorizontal();
        hideOthers = EditorGUILayout.Toggle("隐藏其他", hideOthers);       
        if(mHideOthers != hideOthers)
        {
            mHideOthers = hideOthers;

            HideOtherBlocks(mHideOthers, mShowMesh);
        }

        //显示mesh        
        showMesh = EditorGUILayout.Toggle("显示mesh", showMesh);
        if (mShowMesh != showMesh)
        {
            mShowMesh = showMesh;

            HideOtherBlocks(mHideOthers, mShowMesh);
        }

        GUILayout.EndHorizontal();

        //生成地形阻挡
        GUILayout.BeginHorizontal();
        TerrainBlock terrain = TerrainBlockMgr.Instance.GetTerrainBlock(mCurrentTerrainBlockName);
        if (terrain != null)
        {
            //创建地形阻挡
            if (GUILayout.Button("生成地形阻挡", GUILayout.Width(100)))
            {
                //操作是否结束
                if (!IsCreateTerrainBlockFinished())
                {
                    EditorUtility.DisplayDialog("错误", "地形创建操作需要先完成!", "OK");
                    return;
                }

                terrain.Build();
            }

            //高度            
            terrain.mHeight = EditorGUILayout.FloatField("高度", terrain.mHeight, GUILayout.Width(200));
            //方向
            terrain.mClockWise = EditorGUILayout.Toggle("顺时针", terrain.mClockWise);
        }
        GUILayout.EndHorizontal();


        //导出地形阻挡
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出地形阻挡", GUILayout.Width(100)))
        {
            //当前scene
            UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (scene != null)
            {
                string name = scene.name + "_block";
                TerrainBlockMgr.Instance.Export(name);
            }
        }
        GUILayout.EndHorizontal();

        //保存地形阻挡        
        if (GUILayout.Button("保存地形阻挡", GUILayout.Width(100)))
        {
            //操作是否结束
            if (!IsCreateTerrainBlockFinished())
            {
                EditorUtility.DisplayDialog("错误", "地形创建操作需要先完成!", "OK");
                return;
            }

            UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (scene != null)
            {
                string filePath = Application.dataPath + "/Media/TerrainBlock/" + scene.name + "_block.xml";
                TerrainBlockMgr.Instance.Save(filePath);
            }
        }

        //创建测试角色
        if (GUILayout.Button("创建测试角色", GUILayout.Width(100)))
        {
            //主角色，默认guid 10000       
            SceneEditorWindow.Instance().CreateSimulatePlayer();
        }

        GUILayout.EndVertical();


        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

    }

    //加载地形阻挡
    public void LoadTerrainBlock()
    {
        UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene != null)
        {
            string filePath = Application.dataPath + "/Media/TerrainBlock/" + scene.name + "_block.xml";

            if (File.Exists(filePath))
                TerrainBlockMgr.Instance.Load(filePath);
        }
    }

    //创建地形阻挡
    public void CreateTerrainBlock()
    {
        SceneOperationMgr.Instance.SetCurrentOperation("CreateTerrainBlockHandle", null);
    }

    //编辑地形阻挡
    public void EditTerrainBlock()
    {
        CreateTerrainBlockHandle handle = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
        //还在地形创建阶段
        if (handle != null && !handle.mIsFinished)
        {
            return;
        }
        else
        {
            //设置地形创建阶段
            SceneOperationMgr.Instance.SetCurrentOperation("CreateTerrainBlockHandle", null);
            handle = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
            if (handle != null && mCurrentTerrainBlockName != string.Empty)
            {
                //设置当前Block名称
                handle.mBlockName = mCurrentTerrainBlockName;
                handle.mIsFinished = false;

                TerrainBlock block = TerrainBlockMgr.Instance.GetTerrainBlock(handle.mBlockName);
                if (block != null && block.mKeyObjList.Count != 0)
                {
                    block.mIsEdit = true;
                    Vector3 lastPos = block.mKeyObjList[block.mKeyObjList.Count - 1].transform.position;
                    TerrainBlockMgr.Instance.SetHandlePosition(lastPos);
                    //添加Handle
                    TerrainBlockMgr.Instance.SelectHandle();
                    TerrainBlockMgr.Instance.ShowHandle(true);
                }
            }
        }
    }

    //删除地形阻挡
    public void DeleteTerrainBlock()
    {
        //操作是否结束
        if (!IsCreateTerrainBlockFinished())
        {
            EditorUtility.DisplayDialog("错误", "地形创建操作需要先完成!", "OK");
            return;
        }
        //当前地形块不为空
        if (mCurrentTerrainBlockName != string.Empty)
        {
            TerrainBlockMgr.Instance.RemoveTerrainBlock(mCurrentTerrainBlockName);
        }
        Selection.activeGameObject = null;
    }

    //拆分地形阻挡
    public void SplitTerrainBlock()
    { 
        //操作是否结束
        if (!IsCreateTerrainBlockFinished())
        {
            EditorUtility.DisplayDialog("错误", "地形创建操作需要先完成!", "OK");
            return;
        }

        //需要选中物体
        if (Selection.activeGameObject == null)
            return;

        //确保选中Cube
        GameObject obj = Selection.activeGameObject;
        if (obj.name != "Cube")
            return;

        //获取TerrainBlock名称
        GameObject terainBlockObj = obj.transform.parent.parent.gameObject;
        string blockName = terainBlockObj.name;
        TerrainBlock block = TerrainBlockMgr.Instance.GetTerrainBlock(blockName);
        if (block == null)
            return;

        //获取选中索引
        int selectIndex = block.FindIndex(obj);
        if(selectIndex != -1)
        {
            if(TerrainBlockMgr.Instance.SplitTerrainBlock(block, selectIndex))            
                EditorUtility.DisplayDialog("提示", "地形拆分成功!", "OK");
        }      
    }

    //隐藏其他Block
    public void HideOtherBlocks(bool hideOthers,bool showMesh)
    {
        TerrainBlock block = TerrainBlockMgr.Instance.GetTerrainBlock(mCurrentTerrainBlockName);
        if (block != null)
        {
            TerrainBlockMgr.Instance.HideOtherBlocks(block, hideOthers, showMesh);
        }
    }

    //删除地形关键点
    public void DeleteTerrainKeyPoint()
    {
        //操作是否结束
        if (!IsCreateTerrainBlockFinished())
        {
            EditorUtility.DisplayDialog("错误", "地形创建操作需要先完成!", "OK");
            return;
        }

        //需要选中物体
        if (Selection.activeGameObject == null)
            return;

        //确保选中Cube
        GameObject obj = Selection.activeGameObject;
        if (obj.name != "Cube")
            return;

        //获取TerrainBlock名称
        GameObject terainBlockObj = obj.transform.parent.parent.gameObject;
        string blockName = terainBlockObj.name;
        TerrainBlock block = TerrainBlockMgr.Instance.GetTerrainBlock(blockName);
        if (block == null)
            return;

        //删除选中节点
        block.DeleteKeyObj(obj);
    }

    //创建地形阻挡操作是否结束
    public bool IsCreateTerrainBlockFinished()
    {
        CreateTerrainBlockHandle handle = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
        if (handle != null && !handle.mIsFinished)
        {
            return false;
        }
        return true;
    }
}