using Server._01Service.CfgSvc;
using Server._01Service.NetSvc;
using Server._01Service.TimerSvc;
using Server._02System.BuySys;
using Server._02System.ChatSys;
using Server._02System.FubenSys;
using Server._02System.GuideSys;
using Server._02System.LoginSys;
using Server._02System.PowerSys;
using Server._02System.StrongSys;
using Server._02System.TaskSys;
using Server._03Cache;
using Server._04DB;

namespace Server
{
    public class ServerRoot
    {
        private static ServerRoot instance = null;

        public static ServerRoot Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new ServerRoot();
                }
                return instance;
            }
        }

        public void Init()
        {
            // 数据层
            DBMgr.Instance.Init();

            //服务层
            CacheSvc.Instance.Init();
            NetSvc.Instance.Init();
            CfgSvc.Instance.Init();
            TimerSvc.Instance.Init();

            //业务系统层
            LoginSys.Instance.Init();
            GuideSys.Instance.Init();
            StrongSys.Instance.Init();
            ChatSys.Instance.Init();
            BuySys.Instance.Init();
            PowerSys.Instance.Init();
            TaskSys.Instance.Init();
            FubenSys.Instance.Init();

        }

        public void Update()
        {
            NetSvc.Instance.Update();
            TimerSvc.Instance.Update();
        }

        private int SessionID = 0;
        public int GetSessionID()
        {
            if(SessionID == int.MaxValue)
            {
                SessionID = 0;
            }
            return SessionID += 1;
        }
    }
}
