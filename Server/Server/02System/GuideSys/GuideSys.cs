using PEProtocal;
using Server._01Service.CfgSvc;
using Server._01Service.NetSvc;
using Server._03Cache;
using Server._02System.TaskSys;
using System.Globalization;

namespace Server._02System.GuideSys
{
    public class GuideSys
    {
        private static GuideSys instance = null;

        public static GuideSys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GuideSys();
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
            PECommon.Log("Guide System Init Done");
        }

        public void ReqGuide(MsgPack pack)
        {
            ReqGuide data = pack.msg.reqGuide;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspGuide
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            GuideCfg gc = cfgSvc.GetGuideCfg(data.guideID);

            //更新引导ID
            if(pd.guideID == data.guideID)
            {
                // 检测是否为智者点拨任务
                if(pd.guideID == 1001)
                {
                    TaskSys.TaskSys.Instance.CalcTaskPrgs(pd,1);
                }
                pd.guideID += 1;

                //获取任务奖励
                pd.coin += gc.coin;
                PECommon.CalExp(pd, gc.exp);

                //更新到数据库
                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    //发送给客户端
                    msg.rspGuide = new RspGuide
                    {
                        guideID = pd.guideID,
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                    };
                }
            }
            else
            {
                //服务端数据和客户端数据不匹配，出现异常
                msg.err = (int)ErrorCode.ServerDataError;
            }

            pack.session.SendMsg(msg);
        }

    }
}
