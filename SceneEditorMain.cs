using UnityEngine;
using UnityEditor;
using System.Collections;

//场景编辑器更新主循环
[InitializeOnLoad]
public class SceneEditorMain
{
    static SceneEditorMain()
    {
        EditorApplication.update += EditUpdate;
    }

    static void EditUpdate()
    {
        //确保在编辑模式下
        if (!Application.isPlaying && SceneEditorWindow.mInstance != null)
        {
            float deltaTime = Time.fixedDeltaTime;

            if (SceneEditorWindow.mInstance != null)
            {
                SceneEditorWindow.Instance().Updated(deltaTime);

                //编辑器输入更新
                SceneEditorWindow.Instance().OnSimulatePlayerInput(deltaTime);
            }

            //场景滑索功能更新
            SceneStropMgr.Instance.Updated(deltaTime);
            //场景升降机更新
            SceneLiftMgr.Instance.Updated(deltaTime);

                    
            //角色系统更新
            UnitMgr.Instance.Updated((deltaTime));
            //相机系统更新
            CameraMgr.Instance.Updated(deltaTime);
            //技能系统更新
            SkillMgr.Instance.Update(deltaTime);
            //底层模型系统更新
            DynamicModelMgr.Instance.EditUpdate(deltaTime);

            //缓存操作       
            UnitMgr.Instance.HandleCache(deltaTime);

            //释放操作    
            SkillMgr.Instance.HandleDelete();
            UnitMgr.Instance.HandleDelete();            
        }
    }
}
