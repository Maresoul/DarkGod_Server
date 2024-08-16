using PEProtocal;
using Server._01Service.CfgSvc;
using Server._01Service.NetSvc;
using Server._03Cache;

namespace Server._02System.StrongSys
{
    public class StrongSys
    {
        private static StrongSys instance = null;
        public static StrongSys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StrongSys();
                }
                return instance;
            }
        }
        private CacheSvc cacheSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            PECommon.Log("Strong System Init Down");
        }

        public void ReqStrong(MsgPack pack)
        {
            ReqStrong data = pack.msg.reqStrong;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspStrong,
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

            // 条件判断
            int curtStarLv = pd.strongArr[data.pos];
            StrongCfg nextSd = CfgSvc.Instance.GetStrongCfg(data.pos,curtStarLv+1);
            if (pd.lv < nextSd.minlv)
            {
                msg.err = (int)ErrorCode.LackLevel;
            }
            else if (pd.coin < nextSd.coin)
            {
                msg.err = (int)ErrorCode.LackCoin;
            }
            else if (pd.crystal < nextSd.crystal)
            {
                msg.err = (int)ErrorCode.LackCrystal;
            }
            else
            {
                //任务进度更新
                TaskSys.TaskSys.Instance.CalcTaskPrgs(pd, 3); 
                //资源扣除
                pd.coin -= nextSd.coin;
                pd.crystal -= nextSd.crystal;
                pd.strongArr[data.pos] += 1;

                //增加属性
                pd.hp += nextSd.addhp;
                pd.ad += nextSd.addhurt;
                pd.ap += nextSd.addhurt;
                pd.addef += nextSd.adddef;
                pd.apdef += nextSd.adddef;
            }

            //更新数据库
            if (!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                // 返回客户端
                msg.rspStrong = new RspStrong
                {
                    coin = pd.coin,
                    crystal = pd.crystal,
                    hp = pd.hp,
                    ad = pd.ad,
                    ap = pd.ap,
                    addef = pd.addef,
                    apdef = pd.apdef,
                    strongArr = pd.strongArr
                };
            }

            pack.session.SendMsg(msg);

        }
    }
}
