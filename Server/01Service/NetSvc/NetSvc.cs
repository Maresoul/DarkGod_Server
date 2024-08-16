using PENet;
using PEProtocal;
using Server._02System.BuySys;
using Server._02System.ChatSys;
using Server._02System.FubenSys;
using Server._02System.GuideSys;
using Server._02System.LoginSys;
using Server._02System.StrongSys;
using Server._02System.TaskSys;
using System.Collections;
using System.Collections.Generic;

namespace Server._01Service.NetSvc
{
    public class MsgPack
    {
        public ServerSession session;
        public GameMsg msg;
        public MsgPack(ServerSession session,GameMsg msg)
        {
            this.session = session;
            this.msg = msg;
        }
    }

    public class NetSvc
    {
        private static NetSvc instance = null;

        public static NetSvc Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NetSvc();
                }
                return instance;
            }
        }

        public static readonly string obj = "lock";
        private Queue<MsgPack> msgPackQueue = new Queue<MsgPack>(); 

        public void Init()
        {
            PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
            server.StartAsServer(SrvCfg.srvIP,SrvCfg.srvPort);

            PECommon.Log("Net Service Init Done.");        
        }

        public void AddMsgQue(ServerSession session,GameMsg msg)
        {
            lock (obj)
            {
                msgPackQueue.Enqueue(new MsgPack(session,msg)); 
            }

        }

        public void Update()
        {
            if(msgPackQueue.Count > 0)
            {
                //PECommon.Log("PackCount:" + msgPackQueue.Count);
                lock (obj)
                {
                    MsgPack pack = msgPackQueue.Dequeue();
                    HandOutMsg(pack);
                }
            }
            
        }

        //消息分发到不同系统中处理
        private void HandOutMsg(MsgPack pack)
        {
            switch ((CMD)pack.msg.cmd)
            {
                case CMD.ReqLogin:
                    LoginSys.Instance.ReqLogin(pack); 
                    break;

                case CMD.ReqRename:
                    LoginSys.Instance.ReqRename(pack);
                    break;
                case CMD.ReqGuide:
                    GuideSys.Instance.ReqGuide(pack);
                    break;
                case CMD.ReqStrong:
                    StrongSys.Instance.ReqStrong(pack);
                    break;
                case CMD.SndChat:
                    ChatSys.Instance.SndChat(pack);
                    break;
                case CMD.ReqBuy:
                    BuySys.Instance.ReqBuy(pack);
                    break;
                case CMD.ReqTakeTaskReward:
                    TaskSys.Instance.ReqTakeTaskReward(pack);
                    break;
                case CMD.ReqFBFight:
                    FubenSys.Instance.ReqFBFight(pack);
                    break;
                case CMD.ReqFBFightEnd:
                    FubenSys.Instance.ReqFBFightEnd(pack);
                    break;

            }
        }
    }
}
