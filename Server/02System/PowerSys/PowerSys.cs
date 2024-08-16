using PEProtocal;
using Server._01Service.NetSvc;
using Server._01Service.TimerSvc;
using Server._03Cache;
using System.Collections.Generic;

namespace Server._02System.PowerSys
{
    public class PowerSys
    {
        private static PowerSys instance = null;

        public static PowerSys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PowerSys();
                }
                return instance;
            }
        }

        private CacheSvc cacheSvc = null;
        private TimerSvc timerSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            timerSvc = TimerSvc.Instance;

            TimerSvc.Instance.AddTimeTask(CalPowerAdd,PECommon.PowerAddSpace,PETimeUnit.Second,0);
            PECommon.Log("Power System Init Done");
        }

        private void CalPowerAdd(int tid)
        {
            //计算体力增长
            //PECommon.Log("All Online Player Calc Power Increase");

            GameMsg msg = new GameMsg{
                cmd = (int)CMD.PshPower
            };
            msg.pshPower = new PshPower();

            //所有在线玩家实时体力增长
            Dictionary<ServerSession, PlayerData> onlineDic = cacheSvc.GetOnlineCache();
            foreach(var item in onlineDic)
            {
                PlayerData pd = item.Value;
                ServerSession session = item.Key;

                int powerMax = PECommon.GetPowerLimit(pd.lv);
                if (pd.power >= powerMax)
                {
                    continue;
                }
                else
                {
                    pd.power += PECommon.PowerAddCount;
                    pd.time = timerSvc.GetNowTime();
                    if (pd.power > powerMax)
                        pd.power = powerMax;
                }

                if(!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.pshPower.power = pd.power;
                }
                session.SendMsg(msg);
            }
        }
    }
}
