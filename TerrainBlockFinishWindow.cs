using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

//地形阻挡编辑完成窗口
public class TerrainBlockFinishWindow : EditorWindow
{
    static TerrainBlockFinishWindow m_instance = null;

    public TerrainBlockFinishWindow()
    {
    }

    //窗口成员
    public static TerrainBlockFinishWindow Instance()
    {
        if (null == m_instance)
        {
            m_instance = TerrainBlockFinishWindow.GetWindowWithRect<TerrainBlockFinishWindow>(new Rect(300, 300, 400, 200), true, "TerrainBlockFinishWindow");
        }
        return m_instance;
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        //封闭
        if(GUILayout.Button("闭合"))
        {
            CreateTerrainBlockHandle operation = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
            if(operation != null )
            {
                operation.FinishOperation(true);
            }
            this.Close();
        }
        else if(GUILayout.Button("不闭合"))
        {
            CreateTerrainBlockHandle operation = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
            if (operation != null)
            {
                operation.FinishOperation(false);
            }
            this.Close();
        }
        else if(GUILayout.Button("取消"))
        {
            this.Close();   
        }
        GUILayout.EndHorizontal();
    }
}