using PEProtocal;
using Server._01Service.NetSvc;
using Server._02System.LoginSys;
using Server._04DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server._03Cache
{
    public class CacheSvc
    {
        private static CacheSvc instance = null;

        public static CacheSvc Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CacheSvc();
                }
                return instance;
            }
        }
        private DBMgr dbMgr;

        private Dictionary<string, ServerSession> onLineAcctDic = new Dictionary<string, ServerSession>();
        private Dictionary<ServerSession, PlayerData> onLineSessionDic = new Dictionary<ServerSession, PlayerData>();

        public void Init()
        {
            dbMgr = DBMgr.Instance;
            PECommon.Log("Cache Service Init Done");
        }

        public bool IsAcctOnLine(string acct)
        {
            return onLineAcctDic.ContainsKey(acct);
        }


        public PlayerData GetPlayerData(string acct,string pass)
        {
            
            return dbMgr.QueryPlayerData(acct,pass);
        }


        /// <summary>
        /// 帐号上线，缓存数据
        /// </summary>
        public void AcctOnLine(string acct,ServerSession session,PlayerData playerData)
        {
            onLineAcctDic.Add(acct, session);
            onLineSessionDic.Add(session, playerData);
        }


        public bool IsNameExist(string name)
        {
            return dbMgr.QueryNameData(name);
        }

        public List<ServerSession> GetOnLineServerSession()
        {
            List<ServerSession> lst = new List<ServerSession>();
            foreach(var item in onLineSessionDic)
            {
                lst.Add(item.Key);
            }
            return lst;
        }

        public PlayerData GetPlayerDataBySession(ServerSession session)
        {
            if (onLineSessionDic.TryGetValue(session, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                return null;
            }
        }

        public Dictionary<ServerSession,PlayerData> GetOnlineCache()
        {
            return onLineSessionDic;
        }

        public ServerSession GetOnlineServerSessionByID(int ID)
        {
            ServerSession session = null;
            foreach(var item in onLineSessionDic)
            {
                if (item.Value.id == ID)
                {
                    session = item.Key;
                    break;
                }
            }
            return session;
        }

        public bool UpdatePlayerData(int id,PlayerData playerData)
        {
            return dbMgr.UpdatePlayerData(id,playerData);
        }

        public void AcctOffLine(ServerSession session)
        {
            foreach(var item in onLineAcctDic)
            {
                if(item.Value == session)
                {
                    onLineAcctDic.Remove(item.Key);
                    break;
                }
            }

            bool success = onLineSessionDic.Remove(session);
            PECommon.Log("Session "+session.sessionID+" Offline Result:" + success);
        }


    }
}
