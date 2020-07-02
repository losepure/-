using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;

namespace 系统维护
{
    class PlanManager
    {
        DbManager db;
        MainFrm mainFrm;
        string[] taskArr;
        //创建线程
        Thread autoRunThread;
        Dictionary<string, PlanTask.doSomeThing> methodDic = new Dictionary<string, PlanTask.doSomeThing>();
        Dictionary<string, PlanTask> planTaskDic=new Dictionary<string,PlanTask>();
        System.Threading.Timer threadTimer;
        //主页面控件的更新
        public delegate void UpdateUI(string msg);


        public PlanManager(DbManager db, MainFrm mainFrm)
        {
            this.db = db;
            this.mainFrm = mainFrm;
            this.threadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(method), null, Timeout.Infinite, 1000);
            loadPlanMethod();
        }

        /// <summary>
        /// 开始计划任务
        /// </summary>
        public void start()
        {
            readPlan();
            this.threadTimer.Change(0, 1000);
        }
        /// <summary>
        /// 停止计划任务
        /// </summary>
        public void stop()
        {
            threadTimer.Change(Timeout.Infinite, 1000);
        }


        public void method(object state)
        {
            foreach (KeyValuePair<string,PlanTask> task in planTaskDic)
            {
                task.Value.start();
            }
         
        }

        public UpdateUI netConnectUI;
        /// <summary>
        /// 给计划名赋予执行内容
        /// </summary>
        private void loadPlanMethod() 
        {
            methodDic.Add("检查更新",delegate()
            {
                Tools.checkUpdate(db, mainFrm,true);
            });
            methodDic.Add("iNode激活", delegate()
            {
               
            });
            methodDic.Add("网络连接状态", delegate()
            {
                NetworkDoctorBackObj backObj = this.mainFrm.networkDoctor.checkNetwork();

                if (!backObj.Status)
                {
                    netConnectUI("中断");
                    this.mainFrm.networkDoctor.start();
                }
                else 
                {
                    netConnectUI("良好");
                    this.mainFrm.netStatus_lab.Text = "良好";
                }
            });
            methodDic.Add("获取用户注册信息", delegate()
            {
                Tools.getUserRegInfo(mainFrm.pm.getSelectedNetCard().Ip,db);
            });
      
     
        }
        /// <summary>
        /// 从数据库中读取计划
        /// </summary>
        private void readPlan()
        {
            this.taskArr= db.getCol<string>("planTask","planName");
            foreach (string taskName in taskArr)
	        {
		        DataRow row= db.getRow("planTask","planName",taskName);
                PlanTask planTask = new PlanTask(row);
                if (!planTaskDic.ContainsKey(taskName))
                {
                    planTask.doit += methodDic[taskName];
                    planTaskDic.Add(taskName, planTask);
                }
            
	        }
        }
        /// <summary>
        /// 临时添加新的计划任务
        /// </summary>
        /// <param name="taskName">计划任务名称（来自数据库）</param>
        public void addPlan(string taskName)
        {
            if (!planTaskDic.ContainsKey(taskName)) 
            {
                DataRow row = db.getRow("planTask", "planName", taskName);
                PlanTask planTask = new PlanTask(row);
                planTask.doit+= methodDic[taskName];
                planTaskDic.Add(taskName, planTask);
            }
           
        }
        /// <summary>
        /// 临时移除计划任务
        /// </summary>
        /// <param name="taskName">计划任务名称（来自数据库）</param>
        public void removePlan(string taskName) 
        {
            if (planTaskDic.ContainsKey(taskName))
            {
                planTaskDic.Remove(taskName);
            }
            
        }

    }
}
