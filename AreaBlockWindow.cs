using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AreaBlockWindow : EditorWindow
{
    private static AreaBlockWindow m_Instance = null;
    private string m_CurrentSceneName = string.Empty;
    private GameObject m_BlockContainer = null;
    private GameObject m_CurBlock = null;
    private AreaBlock m_AreaBlock = null;
    private GameObject m_SphereHandle = null;
    private float m_HandleSize = 1f;

    private Vector2 m_ScrollPos = Vector2.zero;
    private string m_CurSelectedBlockName = string.Empty;
    private Color m_OriginGUIColor;

    private bool m_Creating = false;

    public static AreaBlockWindow Instance()
    {
        if(m_Instance == null)
        {
            m_Instance = GetWindow<AreaBlockWindow>("AreaBlockWindow");
            m_Instance.minSize = new Vector2(400, 300);
            m_Instance.Initialize();
        }
        return m_Instance;
    }

    private void Initialize()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        m_CurrentSceneName = EditorSceneManager.GetActiveScene().name;
        GenerateBlockContainer();
    }

    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        DestroyImmediate(m_SphereHandle);

        AreaBlock[] areaBlockComponents = m_BlockContainer.GetComponentsInChildren<AreaBlock>();
        foreach(var comp in areaBlockComponents)
        {
            comp.DestroyEditableObj();
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private void Update()
    {
        if(m_AreaBlock)
        {
            m_AreaBlock.RefreshBlockVertices();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label(string.Format("当前游戏场景名称：{0}", m_CurrentSceneName));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("Box", GUILayout.Width(200));
        GUILayout.Label("功能区域面板：", "textArea");
        if (GUILayout.Button("创建阻挡"))
        {
            CreateAreaBlock();
        }
        if (GUILayout.Button("编辑阻挡"))
        {
            EditAreaBlock();
        }
        if (GUILayout.Button("删除阻挡"))
        {
            DestroyImmediate(m_CurBlock);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.Width(200));
        GUILayout.Label("阻挡区域列表：", "textArea");
        m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, false, true);
        foreach (Transform transform in m_BlockContainer.transform)
        {
            string blockName = transform.name;
            if(m_CurSelectedBlockName != string.Empty && m_CurSelectedBlockName == blockName)
            {
                m_OriginGUIColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                if(GUILayout.Button(blockName))
                {
                    OnSelectBlockButton(transform);
                }
                GUI.backgroundColor = m_OriginGUIColor;
            }
            else
            {
                if(GUILayout.Button(blockName))
                {
                    OnSelectBlockButton(transform);
                    m_CurSelectedBlockName = blockName;
                }
            }
        }
        GUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void CreateAreaBlock()
    {
        SelectSphereHandle();
        m_CurBlock = new GameObject();
        m_CurBlock.transform.parent = m_BlockContainer.transform;
        m_AreaBlock = m_CurBlock.AddComponent<AreaBlock>();
        m_AreaBlock.Initialize();
        m_CurSelectedBlockName = m_CurBlock.name;

        m_Creating = true;
    }

    void EditAreaBlock()
    {
        if(m_AreaBlock)
        {
            m_AreaBlock.RebuildCubeHandles();
            SelectSphereHandle();
        }
    }

    void OnSelectBlockButton(Transform _transform)
    {
        m_CurBlock = _transform.gameObject;
        m_AreaBlock = _transform.GetComponent<AreaBlock>();
        Selection.activeGameObject = _transform.gameObject;
    }

    void SelectSphereHandle()
    {
        if (!m_SphereHandle)
        {
            m_SphereHandle = new GameObject("[Handle]");
        }
        Selection.activeGameObject = m_SphereHandle;
    }

    /// <summary>
    /// 生成阻挡区域容器
    /// </summary>
    void GenerateBlockContainer()
    {
        m_BlockContainer = GameObject.Find("[BlockContainer]");
        if (!m_BlockContainer)
        {
            m_BlockContainer = new GameObject("[BlockContainer]");
        }
    }

    void AddSubBlock()
    {
        if(m_CurBlock)
        {
            m_AreaBlock.AddCubeHandle(m_SphereHandle.transform.position);
        }
    }

    void OnKeyDownEvent(Event curEvent)
    {
        if (curEvent.isKey && curEvent.keyCode == KeyCode.C)
        {
            AddSubBlock();
        }
        else if (curEvent.isKey && curEvent.keyCode == KeyCode.K)
        {
            if (m_AreaBlock)
            {
                m_AreaBlock.DeleteCubeHandle(Selection.activeGameObject);
            }
        }
    }

    void OnLeftMouseDownEvent(Event curEvent)
    {
        if (curEvent.button == 0 && m_Creating)
        {
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(curEvent.mousePosition);
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                Vector3 point = hit.point;
                point.y = 0f;
                m_SphereHandle.transform.position = point;
            }
            m_Creating = false;
        }
    }

    void OnSceneGUI(SceneView view)
    {
        Event current = Event.current;
        switch (current.type)
        {
            case EventType.KeyDown:
                OnKeyDownEvent(current);
                break;
            case EventType.mouseDown:
                OnLeftMouseDownEvent(current);
                break;
        }

        if(m_SphereHandle)
        {
            Transform transform = m_SphereHandle.transform;
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, transform.position, transform.rotation, m_HandleSize, EventType.Repaint);
        }
    }
}
