

using PEProtocal;
using Server._01Service.CfgSvc;
using Server._01Service.NetSvc;
using Server._01Service.TimerSvc;
using Server._03Cache;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Server._02System.TaskSys
{
    public class TaskSys
    {
        private static TaskSys instance = null;

        public static TaskSys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TaskSys();
                }
                return instance;
            }
        }

        private CacheSvc cacheSvc = null;
        private CfgSvc cfgSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;

            PECommon.Log("Task System Init Done");
        }

        public void ReqTakeTaskReward(MsgPack pack)
        {
            ReqTakeTaskReward data = pack.msg.reqTakeTaskReward;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspTakeTaskReward
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

            //根据rid获得配置
            TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(data.rid);
            // 根据rid获得进度
            TaskRewardData trd = CalcTaskRewardData(pd, data.rid);

            if(trd.prgs == trc.count && !trd.take){
                pd.coin += trc.coin;
                PECommon.CalExp(pd, trc.exp);
                trd.take = true;
                //更新任务进度数据
                CalcTaskArr(pd, trd);

                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    RspTakeTaskReward rspTakeTaskReward = new RspTakeTaskReward
                    {
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                        taskArr = pd.taskArr
                    };
                    msg.rspTakeTaskReward = rspTakeTaskReward;
                }
                pack.session.SendMsg(msg);
            }
            else{
                msg.err = (int)ErrorCode.ClientDataError;
            }
        }

        public TaskRewardData CalcTaskRewardData(PlayerData pd,int rid)
        {
            TaskRewardData trd = null;
            for(int i = 0; i < pd.taskArr.Length; i++)
            {
                string[] taskinfo = pd.taskArr[i].Split('|');
                if (int.Parse(taskinfo[0]) == rid)
                {
                    trd = new TaskRewardData
                    {
                        ID = int.Parse(taskinfo[0]),
                        prgs = int.Parse(taskinfo[1]),
                        take = taskinfo[2].Equals("1"),
                    };
                    break;
                }
            }
            return trd;
        }

        public void CalcTaskArr(PlayerData pd, TaskRewardData trd)
        {
            string result = trd.ID + "|" +trd.prgs + "|"+(trd.take ? 1:0);
            int index = -1;
            for(int i=0;i<pd.taskArr.Length; i++)
            {
                string[] taskinfo = pd.taskArr[i].Split('|');
                if (int.Parse(taskinfo[0]) == trd.ID)
                {
                    index = i;
                    break;
                }
            }

            pd.taskArr[index] = result;
        }
        
        public void CalcTaskPrgs(PlayerData pd,int tid)
        {
            TaskRewardData trd = CalcTaskRewardData(pd,tid);
            TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

            if(trd.prgs < trc.count)
            {
                trd.prgs += 1;
                //更新任务进度
                CalcTaskArr(pd, trd);

                ServerSession session =  cacheSvc.GetOnlineServerSessionByID(pd.id);
                if(session != null)
                {
                    session.SendMsg(new GameMsg
                    {
                        cmd = (int)CMD.PshTaskPrgs,
                        pshTaskPrg = new PshTaskPrg
                        {
                            taskArr = pd.taskArr,
                        }
                    });
                }

            }
        }

        public PshTaskPrg GetCalcTaskPrgs(PlayerData pd, int tid)
        {
            TaskRewardData trd = CalcTaskRewardData(pd, tid);
            TaskRewardCfg trc = cfgSvc.GetTaskRewardCfg(tid);

            if (trd.prgs < trc.count)
            {
                trd.prgs += 1;
                //更新任务进度
                CalcTaskArr(pd, trd);

                return new PshTaskPrg {
                    taskArr = pd.taskArr
                };
            }
            else
            {
                return null;
            }
        }
    }
}
