using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Linq;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

//地形阻挡数据管理
public class TerrainBlockMgr :Singleton<TerrainBlockMgr>
{    
    //管理器GameObject
    public GameObject mTerrainBlockMgrObj = null;
    //通用阻挡块ameObject
    public GameObject mHandleBlockObj = null;

    //保存的地形块列表
    public Dictionary<string,TerrainBlock> mTerrainBlockMap = new Dictionary<string,TerrainBlock>();
        
    public TerrainBlockMgr()
    {        
    }

    //每次调用
    public void Init()
    {
        //释放
        Release();
        //重置
        TerrainBlock.mGuid = 0;
        mTerrainBlockMgrObj = new GameObject("TerrainBlockMgr");
        mHandleBlockObj = GameObject.Instantiate(Resources.Load("RCube")) as GameObject;        
    }

    //释放
    public void Release()
    {
        if(mTerrainBlockMgrObj != null)
        {
            for (int i = 0; i < mTerrainBlockMgrObj.transform.childCount; i++)
            {
                GameObject.DestroyImmediate(mTerrainBlockMgrObj.transform.GetChild(i).gameObject);
            }
        }
            
        mTerrainBlockMap.Clear();
    }

    //创建地形块
    public TerrainBlock CreateTerrainBlock()
    {
        //添加
        TerrainBlock block = new TerrainBlock();
        block.Create();
        mTerrainBlockMap.Add(block.mName, block);

        return block;
    }

    //移除地形块
    public void RemoveTerrainBlock(string block)
    {
        GameObject.DestroyImmediate(mTerrainBlockMap[block].mTerrainBlockObj);
        mTerrainBlockMap.Remove(block);
    }

    //根据索引获取地形块
    public TerrainBlock GetTerrainBlock(string name)
    {
        if (mTerrainBlockMap.ContainsKey(name))
            return mTerrainBlockMap[name];
        else
            return null;          
    }

    //在指定位置拆分Block
    public bool SplitTerrainBlock(TerrainBlock block, int index)
    {

        Debug.Log("-----------------------split");

        //点>=3
        if (block.mKeyObjList.Count < 3)
            return false;

        //头尾,不处理
        if (index == 0 || index == block.mKeyObjList.Count - 1)
            return false;

        //头
        TerrainBlock headBlock = new TerrainBlock();
        headBlock.Create();
        headBlock.mHeight = block.mHeight;
        headBlock.mClockWise = block.mClockWise;

        for (int i=0; i<=index; i++)
        {
            headBlock.AddKeyObj(block.mKeyObjList[i].transform.position);
        }
        headBlock.Build();

        //尾
        TerrainBlock taildBlock = new TerrainBlock();
        taildBlock.Create();
        taildBlock.mHeight = block.mHeight;
        taildBlock.mClockWise = block.mClockWise;
        for (int i= index; i < block.mKeyObjList.Count; i++)
        {
            taildBlock.AddKeyObj(block.mKeyObjList[i].transform.position);
        }
        taildBlock.Build();

        mTerrainBlockMap.Add(headBlock.mName, headBlock);
        mTerrainBlockMap.Add(taildBlock.mName, taildBlock);

        RemoveTerrainBlock(block.mName);
        return true;
    }

    //设置Handle阻挡体位置
    public void SetHandlePosition(Vector3 pos)
    {
        mHandleBlockObj.transform.position = pos;
    }

    //选中Handle阻挡体
    public void SelectHandle()
    {
        Selection.activeObject = mHandleBlockObj;        
    }
    
    //设置Handle可见性
    public void ShowHandle(bool flag)
    {
        mHandleBlockObj.SetActive(flag);
    }


    //根据两点位置,以及高度，计算碰撞Plane位置，缩放信息
    public void CalPlaneInfo(Vector3 v1, Vector3 v2, float height,bool clockWisze, ref Vector3 center, ref Vector3 scale, ref Vector3 forward)
    {
        //修正为最低高度
        float minY = Mathf.Min(v1.y, v2.y);
        v1.y = minY;
        v2.y = minY;
        //宽度
        float width = Vector3.Distance(v1, v2);

        //中心
        center = (v1 + v2) / 2.0f;
        center.y += height / 2.0f;

        //缩放
        scale.x = width;
        scale.y = height;
        scale.z = 1.0f;

        Vector3 dir = v2 - v1;
        dir.Normalize();
        
        //朝向设置
        if(clockWisze)
            forward = Vector3.Cross(dir, Vector3.up);
        else
            forward = Vector3.Cross(Vector3.up,dir);
    } 

    //创建所有的地形阻挡体
    public void BuildTerrainBlock()
    {
        foreach (var block in mTerrainBlockMap)
        {
            block.Value.Build();
        }
    }

    //导出地形阻挡数据
    public void Export(string name)
    {
        GameObject exportObj = new GameObject("terrainBlock");
        foreach (var block in mTerrainBlockMap)
        {
            GameObject childObj = new GameObject(block.Value.mName);
            childObj.transform.parent = exportObj.transform;

            //克隆collider
            GameObject cloneCollider = GameObject.Instantiate(block.Value.mCollider);
            cloneCollider.name = "collider";
            cloneCollider.transform.parent = childObj.transform;

            //取消Mesh             
            MeshRenderer[] renders = cloneCollider.GetComponentsInChildren<MeshRenderer>();
            foreach(var render in renders)
            {
                render.enabled = false;
            }
        }
      
        //导出prefab
        string path = "Assets/Prefab/TerrianBlock/Resources/"+ name +".prefab";
        PrefabUtility.CreatePrefab(path, exportObj);

        GameObject.DestroyImmediate(exportObj);
        //提示
        EditorUtility.DisplayDialog("提示", "导出地形碰撞预支件成功!"+ path, "OK");
    }

    //保存地形阻挡信息
    public void Save(string fileName)
    {
        //XML头文件
        XmlDocument xmldoc = new XmlDocument();
        XmlDeclaration xmldecl = xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null);
        xmldoc.AppendChild(xmldecl);
        XmlElement terrainBlockNode = xmldoc.CreateElement("terrain_block");
        xmldoc.AppendChild(terrainBlockNode);

        //遍历block列表
        foreach (var pair in mTerrainBlockMap)
        {
            TerrainBlock block = pair.Value;
            //添加block
            string name = "block" + pair.Key;
            XmlElement blockNode = xmldoc.CreateElement(name);
            terrainBlockNode.AppendChild(blockNode);

            //保存block信息
            blockNode.SetAttribute("height", block.mHeight.ToString("f3"));
            blockNode.SetAttribute("clock_wise", block.mClockWise.ToString());
            //保存点
            string str = string.Empty;
            for (int i=0; i < block.mKeyObjList.Count; i++)
            {
                str += CommonUtils.Vector3ToString(block.mKeyObjList[i].transform.position);

                if (i != block.mKeyObjList.Count - 1)
                    str += "|";              
            }
            blockNode.SetAttribute("pt_list", str);
        }
        xmldoc.Save(fileName);
    }

    //加载地形阻挡信息
    public void Load(string fileName)
    {
        //读取xml            
        XmlDocument xmldoc = new XmlDocument();

        try
        {
            xmldoc.Load(fileName);
        }
        catch (Exception e)
        {
            Debug.LogError(fileName + e.StackTrace + e.Message);
            return;
        }

        //清除 
       Release();

        //地形block
        XmlNode terrainBlockNode = xmldoc.DocumentElement as XmlNode;


        int maxGuid = 0;
        //遍历block子节点
        foreach (XmlNode blockNode in terrainBlockNode.ChildNodes)
        {
            TerrainBlock block = new TerrainBlock();
            string idStr = blockNode.Name.Replace("block", "");
            block.mID = int.Parse(idStr);
            block.mName = idStr;
            block.mHeight = float.Parse((blockNode as XmlElement).GetAttribute("height"));
            block.mClockWise = bool.Parse((blockNode as XmlElement).GetAttribute("clock_wise"));
            //创建
            block.Create();
            //添加点
            string ptListStr = (blockNode as XmlElement).GetAttribute("pt_list");
            string[] ptLists = ptListStr.Split('|');
            foreach(var str in ptLists)
            {
                Vector3 pt = CommonUtils.StringToVector3(str);
                block.AddKeyObj(pt);
            }
            block.Build();

            maxGuid = Math.Max(block.mID, block.mID);

            TerrainBlockMgr.Instance.mTerrainBlockMap.Add(block.mName, block);
        }
        TerrainBlock.mGuid = maxGuid;
    }

    //隐藏其他Block
    public void HideOtherBlocks(TerrainBlock terrainBlock, bool hideOthers,bool showMesh)
    {
     
       foreach(var pair in mTerrainBlockMap)
       {
            TerrainBlock block = pair.Value;
            if(block == terrainBlock)
            {
                block.mTerrainBlockObj.SetActive(true);
                if(showMesh)
                    ShowMeshRender(block.mCollider, true);
                else
                    ShowMeshRender(block.mCollider, false);
                block.mShow = true;
            }
            else
            {
                if (hideOthers)
                {
                    block.mTerrainBlockObj.SetActive(false);                                       
                    block.mShow = false;
                }                    
                else
                {
                    block.mTerrainBlockObj.SetActive(true); 
                    if(showMesh)
                        ShowMeshRender(block.mCollider, true);
                    else
                        ShowMeshRender(block.mCollider, false);
                    block.mShow = true;
                }                    
            }
        }
    }

    public void ShowMeshRender(GameObject obj,bool flag)
    {
       MeshRenderer[] renders = obj.GetComponentsInChildren<MeshRenderer>();
       foreach(var render in renders)
        {
            render.enabled = flag;
        }
    }

    //绘制
    public void OnSceneGUI()
    {
        foreach(var block in mTerrainBlockMap)
        {
            block.Value.OnSceneGUI();
        }
    }
}

