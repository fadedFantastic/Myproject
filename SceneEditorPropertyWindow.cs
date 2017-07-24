using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

//场景编辑属性窗口
public class SceneEditorPropertyWindow : EditorWindow
{
    static SceneEditorPropertyWindow m_instance = null;

    public SceneEditorPropertyWindow()
    {
    }

    //窗口成员
    public static SceneEditorPropertyWindow Instance()
    {
        if (null == m_instance)
        {
            m_instance = EditorWindow.GetWindow<SceneEditorPropertyWindow>("SceneEditorPropertyWindow");            
        }
        return m_instance;
    }

    public string mLiftName;
    void OnGUI()
    {      
        SceneEditorWindow.Instance().PropertyUI();


        

        //GUILayout.BeginHorizontal();
        ////封闭
        //if (GUILayout.Button("闭合"))
        //{
        //    CreateTerrainBlockHandle operation = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
        //    if (operation != null)
        //    {
        //        operation.FinishOperation(true);
        //    }
        //    this.Close();
        //}
        //else if (GUILayout.Button("不闭合"))
        //{
        //    CreateTerrainBlockHandle operation = SceneOperationMgr.Instance.GetCurrentOperation() as CreateTerrainBlockHandle;
        //    if (operation != null)
        //    {
        //        operation.FinishOperation(false);
        //    }
        //    this.Close();
        //}
        //else if (GUILayout.Button("取消"))
        //{
        //    this.Close();
        //}
        //GUILayout.EndHorizontal();
        //this.Repaint();
    }

    
}