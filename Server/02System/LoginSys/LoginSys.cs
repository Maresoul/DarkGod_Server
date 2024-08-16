using PEProtocal;
using Server._01Service.NetSvc;
using Server._01Service.TimerSvc;
using Server._03Cache;
using System.Security.Cryptography;

namespace Server._02System.LoginSys
{
    public class LoginSys
    {
        private static LoginSys instance = null;

        public static LoginSys Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginSys();
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
            PECommon.Log("Login System Init Done");
        }

        public void ReqLogin(MsgPack pack)
        {
            ReqLogin data = pack.msg.reqLogin;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspLogin,
            };

            //当前账号是否上线
            if (cacheSvc.IsAcctOnLine(data.acct))
            {
                //已上线：返回错误信息
                msg.err = (int)ErrorCode.AcctIsOnline;
            }
            else
            {
                /*未上线：
                *账号是否存在
                * 存在：检测密码
                * 不存在：创建默认账号密码
                */

                PlayerData playerData = cacheSvc.GetPlayerData(data.acct, data.pass);
                if(playerData == null)
                {
                    msg.err = (int)ErrorCode.WrongPass;
                }
                else
                {
                    //计算离线体力增长
                    int power = playerData.power;
                    long now = timerSvc.GetNowTime();
                    long millseconds = now - playerData.time;
                    int addPower = (int)(millseconds / (1000 * PECommon.PowerAddSpace)) * PECommon.PowerAddCount;
                    
                    if (addPower > 0)
                    {
                        int powerMax = PECommon.GetPowerLimit(playerData.lv);
                        if (playerData.power < powerMax)
                        {
                            playerData.power += addPower;
                            if(playerData.power > powerMax)
                            {
                                playerData.power = powerMax;
                                PECommon.Log("离线期间体力增长:" + powerMax);
                            }
                            else
                            {
                                PECommon.Log("离线期间体力增长:" + addPower);
                            }
                        }
                    }

                    if(power != playerData.power)
                    {
                        cacheSvc.UpdatePlayerData(playerData.id, playerData);
                    }

                    msg.rspLogin = new RspLogin
                    {
                        playerData = playerData
                    };
                    cacheSvc.AcctOnLine(data.acct, pack.session, playerData);
                }

            }
            //回应客户端
            pack.session.SendMsg(msg);

        }

        public void ReqRename(MsgPack pack)
        {
            ReqRename data = pack.msg.reqRename;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspRename
            };

            // 名字是否存在
            //存在：返回错误码
            //不存在：更新缓存，以及数据库，返回客户端

            if (cacheSvc.IsNameExist(data.name))
            {
                msg.err = (int)ErrorCode.NameIsExist;
            }
            else
            {
                PlayerData playerData = cacheSvc.GetPlayerDataBySession(pack.session);
                playerData.name = data.name;

                if (!cacheSvc.UpdatePlayerData(playerData.id, playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.rspRename = new RspRename
                    {
                        name = data.name,
                    };
                }
                pack.session.SendMsg(msg);
            }

        }

        public void ClearOfflineData(ServerSession session)
        {
            //写入下线时间
            PlayerData pd = cacheSvc.GetPlayerDataBySession(session);
            if(pd != null)
            {
                pd.time = timerSvc.GetNowTime();
                if (!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    PECommon.Log("Update Offline Time Error", LogType.Error); ;
                }
                cacheSvc.AcctOffLine(session);
            }
        }
    }
}
