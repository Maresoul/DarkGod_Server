/*
 * 网络会话连接
 */

using PENet;
using PEProtocal;
using Server._02System.LoginSys;

namespace Server._01Service.NetSvc
{
    public class ServerSession : PESession<GameMsg>
    {
        public int sessionID = 0;

        protected override void OnConnected()
        {
            sessionID = ServerRoot.Instance.GetSessionID();
            PECommon.Log("SessionID:" + sessionID + " Client Connected");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PECommon.Log("SessionID:" + sessionID + " RcvPack CMD" + ((CMD)msg.cmd).ToString());
            NetSvc.Instance.AddMsgQue(this,msg);
        }


        protected override void OnDisConnected()
        {
            LoginSys.Instance.ClearOfflineData(this);
            PECommon.Log("SessionID:" + sessionID + " Client DisConnected");
        }
    }

}
