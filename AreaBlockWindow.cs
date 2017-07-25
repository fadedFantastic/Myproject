using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AreaBlockWindow : EditorWindow
{
    private static AreaBlockWindow m_Instance = null;
    private string m_CurrentSceneName = string.Empty;
    private GameObject m_Handle = null;
    private float m_HandleSize = 1f;
    private GameObject m_BlockGO = null;

    public static AreaBlockWindow Instance()
    {
        if(m_Instance == null)
        {
            m_Instance = GetWindow<AreaBlockWindow>("AreaBlockWindow");
            m_Instance.minSize = new Vector2(400, 300);
        }
        return m_Instance;
    }

    private void Awake()
    {
        m_CurrentSceneName = EditorSceneManager.GetActiveScene().name;
        GenerateBlockContainer();
    }

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        DestroyImmediate(m_Handle);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label(string.Format("当前游戏场景名称：{0}", m_CurrentSceneName));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("Box", GUILayout.Width(200));
        if(GUILayout.Button("加载阻挡"))
        {

        }
        if (GUILayout.Button("创建阻挡"))
        {
            GenerateHandle();
        }
        if (GUILayout.Button("修改阻挡"))
        {

        }
        if (GUILayout.Button("删除阻挡"))
        {

        }
        EditorGUILayout.EndVertical();
    }

    void GenerateHandle()
    {
        if (!m_Handle)
        {
            m_Handle = new GameObject("[Handle]");
        }
        Selection.activeGameObject = m_Handle;
    }

    void GenerateBlockContainer()
    {
        m_BlockGO = GameObject.Find("[BlockContainer]");
        if (!m_BlockGO)
        {
            m_BlockGO = new GameObject("[BlockContainer]");
        }
    }

    void CreateBlock()
    {
        
    }

    void OnSceneGUI(SceneView view)
    {
        Event current = Event.current;
        switch (current.type)
        {
            case EventType.keyDown:
                if (current.isKey && current.keyCode == KeyCode.C)
                    CreateBlock();
                break;
        }

        if(m_Handle)
        {
            Transform transform = m_Handle.transform;
            Handles.color = Color.red;
            Handles.SphereHandleCap(0, transform.position, transform.rotation, m_HandleSize, EventType.Repaint);
        }
    }
}
