﻿using MySql.Data;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Server._01Service.CfgSvc
{
    public class CfgSvc
    {
        private static CfgSvc instance = null;

        public static CfgSvc Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CfgSvc();
                }
                return instance;
            }
        }

        public void Init()
        {
            InitGuideCfg();
            InitStrongCfg();
            InitTaskRewardCfg();
            InitMapCfg();
            PECommon.Log("CfgSvc Init Done");
        }

        #region 自动引导配置

        private Dictionary<int, GuideCfg> guideDic = new Dictionary<int, GuideCfg>();
        private void InitGuideCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"E:\Unity_Project\DarkGod\Assets\Resources\ResCfgs\guide.xml");

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                GuideCfg aGC = new GuideCfg()
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "coin":
                            aGC.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            aGC.exp = int.Parse(e.InnerText);
                            break;

                    }
                }
                guideDic.Add(ID, aGC);
            }
            PECommon.Log("GuideCfg Init Done");
        }

        public GuideCfg GetGuideCfg(int id)
        {
            GuideCfg agc = null;
            if (guideDic.TryGetValue(id, out agc))
            {
                return agc;
            }
            return null;
        }

        #endregion

        #region 强化升级配置
        private Dictionary<int, Dictionary<int, StrongCfg>> strongDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
        private void InitStrongCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"E:\Unity_Project\DarkGod\Assets\Resources\ResCfgs\strong.xml");

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sd = new StrongCfg()
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            sd.pos = val;
                            break;
                        case "starlv":
                            sd.starlv = val;
                            break;
                        case "addhp":
                            sd.addhp = val;
                            break;
                        case "addhurt":
                            sd.addhurt = val;
                            break;
                        case "adddef":
                            sd.adddef = val;
                            break;
                        case "minlv":
                            sd.minlv = val;
                            break;
                        case "coin":
                            sd.coin = val;
                            break;
                        case "crystal":
                            sd.crystal = val;
                            break;
                    }
                }

                Dictionary<int, StrongCfg> dic = null;
                if (strongDic.TryGetValue(sd.pos, out dic))
                {
                    dic.Add(sd.starlv, sd);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sd.starlv, sd);

                    strongDic.Add(sd.pos, dic);
                }

            }
            PECommon.Log("StrongCfg Init Done");

        }

        public StrongCfg GetStrongCfg(int pos, int starlv)
        {
            StrongCfg sd = null;
            Dictionary<int, StrongCfg> dic = null;
            if (strongDic.TryGetValue(pos, out dic))
            {
                if (dic.ContainsKey(starlv))
                {
                    sd = dic[starlv];
                }
            }
            return sd;
        }

        #endregion

        #region 任务奖励配置

        private Dictionary<int, TaskRewardCfg> taskRewardDic = new Dictionary<int, TaskRewardCfg>();
        private void InitTaskRewardCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"E:\Unity_Project\DarkGod\Assets\Resources\ResCfgs\task.xml");

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                TaskRewardCfg trc = new TaskRewardCfg()
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "count":
                            trc.count = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            trc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            trc.exp = int.Parse(e.InnerText);
                            break;

                    }
                }
                taskRewardDic.Add(ID, trc);
            }
            PECommon.Log("TaskRewardCfg Init Done");
        }

        public TaskRewardCfg GetTaskRewardCfg(int id)
        {
            TaskRewardCfg trc = null;
            if (taskRewardDic.TryGetValue(id, out trc))
            {
                return trc;
            }
            return null;
        }

        #endregion

        #region 地图数据配置

        private Dictionary<int, MapCfg> mapDic = new Dictionary<int, MapCfg>();
        private void InitMapCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"E:\Unity_Project\DarkGod\Assets\Resources\ResCfgs\map.xml");

            XmlNodeList nodLst = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodLst.Count; i++)
            {
                XmlElement ele = nodLst[i] as XmlElement;

                if (ele.GetAttributeNode("ID") == null)
                {
                    continue;
                }
                int ID = Convert.ToInt32(ele.GetAttributeNode("ID").InnerText);
                MapCfg mc = new MapCfg()
                {
                    ID = ID
                };

                foreach (XmlElement e in nodLst[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "power":
                            mc.power = int.Parse(e.InnerText);
                            break;
                        case "coin":
                            mc.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            mc.exp = int.Parse(e.InnerText);
                            break;
                        case "crystal":
                            mc.crystal = int.Parse(e.InnerText);
                            break;

                    }
                }
                mapDic.Add(ID, mc);
            }
            PECommon.Log("MapCfg Init Done");
        }

        public MapCfg GetMapCfg(int id)
        {
            MapCfg mc = null;
            if (mapDic.TryGetValue(id, out mc))
            {
                return mc;
            }
            return null;
        }

        #endregion
    }

    public class MapCfg : BaseData<MapCfg>
    {
        public int power;
        public int coin;
        public int exp;
        public int crystal;
    }

    public class TaskRewardCfg : BaseData<TaskRewardCfg>
    {
        public int count;
        public int coin;
        public int exp;
    }

    public class TaskRewardData : BaseData<TaskRewardData>
    {
        public int prgs;
        public bool take;
    }

    public class StrongCfg : BaseData<StrongCfg>
    {
        public int pos;
        public int starlv;
        public int addhp;
        public int addhurt;
        public int adddef;
        public int minlv;
        public int coin;
        public int crystal;
    }

    public class GuideCfg : BaseData<GuideCfg>
    {
        public int coin;
        public int exp;
    }

    public class BaseData<T>
    {
        public int ID;
    }

}
