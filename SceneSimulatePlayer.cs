//using UnityEngine;
//using System.Collections;
////using Google.ProtocolBuffers;
////using NetProto;

////玩家逻辑类
//public class SceneSimulatePlayer : Player
//{
//    public SceneSimulatePlayer()
//    {
//        mUnitType = UNIT_TYPE.UT_SimulatePlayer;

//    }

//    //初始化
//    public override void Create(int unitId, DynamicModel model)
//    {
//        base.Create(unitId, model);
//        //运动组件
//        mUnitController = new UnitController(GetModelObj().GetComponent<CharacterController>(), this);

//        //重力组件
//        GravityController gravityController = new GravityController();
//        mUnitController.mControllerList.Add(ControllerType.Gravity, gravityController);

//        //保存组件
//        mTransform = GetModelObj().transform;
//        //创建怪物状态机
//        mStateMachine = new StateMachine<Player>(this);

//        //初始角色属性
//        InitProperty();

//        //根据类型创建状态机
//        CreateStateMachine();
//    }
//}


