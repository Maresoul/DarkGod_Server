using PEProtocal;
using Server._01Service.NetSvc;
using Server._03Cache;

namespace Server._02System.BuySys
{
    public class BuySys
    {
        private static BuySys instance = null;

        public static BuySys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BuySys();
                }
                return instance;
            }
        }

        private CacheSvc cacheSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            PECommon.Log("Buy System Init Done");
        }

        public void ReqBuy(MsgPack pack)
        {
            ReqBuy data = pack.msg.reqBuy;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspBuy, 
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

            if(pd.diamond < data.cost)
            {
                msg.err = (int)ErrorCode.LackDiamond;
            }
            else
            {
                pd.diamond -= data.cost;
                PshTaskPrg pshTaskPrg = null;
                switch (data.type)
                {
                    case 0:
                        //任务进度更新
                        pshTaskPrg = TaskSys.TaskSys.Instance.GetCalcTaskPrgs(pd, 4);
                        pd.power += 100;
                        break;
                    case 1:
                        //任务进度更新
                        pshTaskPrg = TaskSys.TaskSys.Instance.GetCalcTaskPrgs(pd, 5);
                        pd.coin += 1000;
                        break;
                }

                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    RspBuy rspBuy = new RspBuy
                    {
                        type = data.type,
                        diamond = pd.diamond,
                        coin = pd.coin,
                        power = pd.power,
                    };
                    msg.rspBuy = rspBuy;

                    //并包处理
                    msg.pshTaskPrg = pshTaskPrg;
                }
            }
            pack.session.SendMsg(msg);
        }
    }
}
