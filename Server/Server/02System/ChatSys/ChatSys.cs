using PEProtocal;
using Server._01Service.NetSvc;
using Server._03Cache;
using System.Collections.Generic;

namespace Server._02System.ChatSys
{
    public class ChatSys
    {
        private static ChatSys instance = null;

        public static ChatSys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChatSys();
                }
                return instance;
            }
        }

        private CacheSvc cacheSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            PECommon.Log("Chat System Init Done");
        }

        public void SndChat(MsgPack pack)
        {
            SndChat data = pack.msg.sndChat;
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

            //任务进度更新
            TaskSys.TaskSys.Instance.CalcTaskPrgs(pd, 6);

            GameMsg msg = new GameMsg()
            {
                cmd = (int)CMD.PshChat,
                pshChat = new PshChat
                {
                    name = pd.name,
                    chat = data.chat,
                }
            };

            //广播所有在线客户端
            List<ServerSession> lst = cacheSvc.GetOnLineServerSession();
            byte[] bytes = PENet.PETool.PackNetMsg(msg);
            for(int i = 0; i < lst.Count; i++)
            {
                lst[i].SendMsg(bytes);
            }
        }
    }
}
