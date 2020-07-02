using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace 系统维护
{
    class PlanTask
    {
        private string taskName;

        public string TaskName
        {
            get { return taskName; }
            set { taskName = value; }
        }

        private bool isDueTime = false;
        private int dueTime;

        public int DueTime
        {
            get { return dueTime; }
            set { dueTime = value; }
        }
        private int interVal;

        public int InterVal
        {
            get { return interVal; }
            set { interVal = value; }
        }
        private string staticTime;

        public string StaticTime
        {
            get { return staticTime; }
            set { staticTime = value; }
        }
        private bool isOnce=false;
        private bool onlyOnce;

        public bool OnlyOnce
        {
            get { return onlyOnce; }
            set { onlyOnce = value; }
        }
        private string introduce;

        public string Introduce
        {
            get { return introduce; }
            set { introduce = value; }
        }
        private bool isRunning = false;

        private int ljSecond=0;

        public int LjSecond
        {
            get { return ljSecond; }
            set { ljSecond = value; }
        }

        private void ljReset()
        {
            ljSecond=0;
        }


        public PlanTask(DataRow row) 
        {
            this.taskName =Convert.ToString(row["planName"]);
            this.dueTime = Convert.ToInt32(row["dueTime"]);
            this.interVal = Convert.ToInt32(row["interval"]);
            this.staticTime = Convert.ToString(row["staticTime"]);
            this.onlyOnce = Convert.ToBoolean(row["onlyOnce"]);
            this.introduce = Convert.ToString(row["introduce"]);
        }

        public delegate void doSomeThing();
        public doSomeThing doit;

        public void start() 
        {
            if(isRunning) return;
            isRunning = true;
            //只做一次的情况
            if (onlyOnce == true && isOnce == false) 
            {
                if (ljSecond>=dueTime) 
                {
                    doit();
                    isOnce = true;
                }

            }
            else if (onlyOnce == false)
            {
                if (this.taskName == "检查更新")
                {
                    int i = ljSecond;

                }
                //做多次的情况，分为间隔做和定点做
                if (ljSecond >= dueTime && dueTime>0 && isDueTime == false)
                {
                    isDueTime = true;
                    doit();
                    ljReset();
                }
                else if (interVal > 0 && ljSecond >= interVal && isDueTime) //间隔做
                {
                    doit();
                    ljReset();
                }
                else if (staticTime != "" && staticTime == DateTime.Now.ToShortTimeString().ToString()) //定点做
                {
                    doit();
                }
            }
            ljSecond++;
            isRunning = false;
        }







    }
}
