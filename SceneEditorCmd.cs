using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//场景编辑编辑器命令处理
public class SceneEditorCmd : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [MenuItem("Window/SceneEditor")]
    static void Init1()
    {
        //EngineCfg.Instance.cacheEffect = false;

        ////初始化命名信息
        //ActionNameIdMgr.Instance.Init();

        ////获取GameManager,并设置为Editor模式
        ////ModifyGameManager();

        ////创建模型root节点
        //CreateDynamicModelRootGameObject();

        ////创建特效root节点
        //CreateEffectRootGameObject();
        ////创建声音root节点
        //CreateAudioRootGameObject();

        ////创建特效SpinAxisSphere节点
        //CreateEffectSpinAxisSphereGameObject();
        ////创建声音SpinAxisSphere节点
        //CreateSoundSpinAxisSphereGameObject();

        ////清除数据
        //EffectMgr.Instance.DestoryAllEffect();
        //SkillUnitMgr.Instance.DestoryAllSkill();
        //UnitMgr.Instance.DestoryAll();
        SceneOperationMgr.Instance.SetCurrentOperation("NULL", null);


        ////初始化
        //ModelEditorWindow.Instance().Initlialize();
        //ModelEditorWindow.Instance().Show();
        //SkillTreeWindow.Instance().Show();
        //SkillPropertyWindow.Instance().Show();
        EditorApplication.ExecuteMenuItem("Window/Layouts/SceneEditor");

        //初始化
        SceneEditorWindow.Instance().Initlialize();



        //显示
        SceneEditorWindow.Instance().Show();
        SceneEditorPropertyWindow.Instance().Show();
    }
}

