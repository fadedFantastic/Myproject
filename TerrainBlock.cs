using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


//地形阻挡块
public class TerrainBlock
{
    public static int mGuid = 0;
    //ID
    public int mID = 0;
    //名称
    public string mName = string.Empty;

    //地形阻挡块对应的GameObject
    public GameObject mTerrainBlockObj = null;
    public GameObject mEditObj = null;
    public GameObject mCollider = null;

    //高度
    public float mHeight = 6.0f;
    //方向
    public bool mClockWise = true;

    //是否显示
    public bool mShow = true;


    //用于绘制
    public List<Vector3> mGUIPtList = new List<Vector3>();
    //是否编辑状态
    public bool mIsEdit = false;
    //是否锁住
    public bool mIsLock = false;

    public TerrainBlock()
    {
        mGuid++;
        mID = mGuid;
        mName =/* "TerrainBlock" + */mID.ToString();
    }

    ~TerrainBlock()
    {
        
    }

    //阻挡列表数据
    public List<GameObject> mKeyObjList = new List<GameObject>();

    //创建
    public void Create()
    {            
        mTerrainBlockObj = new GameObject(mName);
        mTerrainBlockObj.transform.parent = TerrainBlockMgr.Instance.mTerrainBlockMgrObj.transform;

        mEditObj = new GameObject("edit");
        mCollider = new GameObject("collider");
        mEditObj.transform.parent = mTerrainBlockObj.transform;
        mCollider.transform.parent = mTerrainBlockObj.transform;
    }

    //添加关键点
    public void AddKeyObj(Vector3 pos)
    {
        //判断是否共点
        if(mKeyObjList.Count != 0)
        {
            //共点，返回
            if(Vector3.Distance(mKeyObjList[mKeyObjList.Count-1].transform.position, pos) < 0.001f)
                return; 
        }

        //创建关键点物体
        GameObject obj = GameObject.Instantiate(Resources.Load("Cube")) as GameObject;
        obj.name = "Cube";
        obj.transform.position = pos;
        obj.transform.parent = mEditObj.transform;
        mKeyObjList.Add(obj);   
    }

    //删除关键点
    public void DeleteKeyObj(GameObject gameObj)
    {
    
        if (mKeyObjList.Count >=3)
        {            
            mKeyObjList.Remove(gameObj);
            GameObject.DestroyImmediate(gameObj);
        }
    }
    
    //刷新绘制点
    public void RefreshGUIPtList()
    {     
        mGUIPtList.Clear();
        for (int i = 0; i < mKeyObjList.Count; i++)
        {
            mGUIPtList.Add(mKeyObjList[i].transform.position);
        }

        //编辑状态
        if(mIsEdit)
        {
            //添加阻挡点
            mGUIPtList.Add(TerrainBlockMgr.Instance.mHandleBlockObj.transform.position);
        }       
    }


    //在编辑器绘制
    public void OnSceneGUI()
    {
        if (!mShow)
            return;

        //刷新绘制点
        RefreshGUIPtList();

        Handles.color = Color.yellow;
        Handles.DrawAAPolyLine(mGUIPtList.ToArray());
    }

    //创建Block
    public void Build()
    {
        //移除已经存在的碰撞Plane
        for(int i=0; i < mCollider.transform.childCount; i++)
        {
            GameObject.Destroy(mCollider.transform.GetChild(i)); 
        }
        

        Vector3 center = Vector3.zero;
        Vector3 scale = Vector3.one;
        Vector3 forward = Vector3.zero;

        for (int i=0; i < mKeyObjList.Count-1; i++)
        {
            Vector3 v1 = mKeyObjList[i].transform.position;
            Vector3 v2 = mKeyObjList[i + 1].transform.position;

            //根据点位置计算碰撞Plane的信息
            TerrainBlockMgr.Instance.CalPlaneInfo(v1, v2, mHeight, mClockWise, ref center, ref scale, ref forward);

            //创建Plane
            GameObject planeObj = GameObject.Instantiate(Resources.Load("Plane")) as GameObject;
            planeObj.name = "Plane";
            planeObj.transform.localPosition = center;
            planeObj.transform.localScale = scale;
            planeObj.transform.forward = forward;

            //绑定到collider
            planeObj.transform.parent = mCollider.transform;
        }
        //设置当前选中为地形块
        Selection.activeGameObject = mTerrainBlockObj;
    }

    //根据选中的GameObject返回其索引值
    public int FindIndex(GameObject gameObj)
    {
        for(int i=0; i < mKeyObjList.Count; i++)
        {
            if (mKeyObjList[i] == gameObj)
                return i;
        }
        return -1;
    }


}
