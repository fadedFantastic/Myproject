using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBlock : MonoBehaviour
{
    private static int m_InstanceID = 0;

    [SerializeField] private List<GameObject> m_Blocks = new List<GameObject>();
    [SerializeField] private List<Vector3> m_BlockVertices = new List<Vector3>();
    public List<Vector3> BlockVertices { get { return m_BlockVertices; } }

    private GameObject m_EditObj;
    private Vector3 m_GizmosSize = Vector3.one;
    [SerializeField][HideInInspector] private bool m_Editing = true;

    public void Initialize()
    {
        ++m_InstanceID;
        gameObject.name = "Block" + m_InstanceID.ToString();
        m_EditObj = new GameObject("Edit");
        m_EditObj.transform.parent = gameObject.transform;
    }

    public void AddCubeHandle(Vector3 pos)
    {
        int count = m_Blocks.Count;
        if(count > 0)
        {
            if(Vector3.Distance(m_Blocks[count - 1].transform.position, pos) < 0.001f)
            {
                return;
            }
        }

        InstantiateCubeHandle(pos);
    }

    public void DeleteCubeHandle(GameObject go)
    {
        if(go.name == "Cube")
        {
            m_Blocks.Remove(go);
            DestroyImmediate(go);
        }
    }

    /// <summary>
    /// 刷新阻挡区域顶点
    /// </summary>
    public void RefreshBlockVertices()
    {
        if (!m_Editing) return;

        m_BlockVertices.Clear();
        foreach(var go in m_Blocks)
        {
            if(go)
            {
                m_BlockVertices.Add(go.transform.position);
            }
        }
    }

    /// <summary>
    /// 重建白色立方体，允许编辑阻挡区域
    /// </summary>
    public void RebuildCubeHandles()
    {
        if (m_EditObj) return;

        m_EditObj = new GameObject("edit");
        m_EditObj.transform.parent = gameObject.transform;

        int size = m_BlockVertices.Count;
        for (int i = 0; i < size; ++i)
        {
            InstantiateCubeHandle(m_BlockVertices[i]);
        }

        m_Editing = true;
    }

    /// <summary>
    /// 创建白色立方体
    /// </summary>
    private void InstantiateCubeHandle(Vector3 pos)
    {
        GameObject go = Instantiate(Resources.Load("Cube")) as GameObject;
        go.name = "Cube";
        go.transform.position = pos;
        go.transform.parent = m_EditObj.transform;
        m_Blocks.Add(go);

        if(m_Editing)
        {
            m_BlockVertices.Add(go.transform.position);
        }
    }

    /// <summary>
    /// 销毁可编辑物体
    /// </summary>
    public void DestroyEditableObj()
    {
        m_Editing = false;
        m_Blocks.Clear();
        DestroyImmediate(m_EditObj);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        int size = m_BlockVertices.Count;
        for (int i = 0; i < size - 1; ++i)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_BlockVertices[i], m_BlockVertices[i + 1]);
        }
        if (size > 1)
        {
            Gizmos.DrawLine(m_BlockVertices[0], m_BlockVertices[size - 1]);
        }

        // 非编辑模式下辅助显示
        if(!m_Editing)
        {
            for (int i = 0; i < size; ++i)
            {
                Gizmos.DrawCube(m_BlockVertices[i], m_GizmosSize);
            }
        }
    }
#endif
}