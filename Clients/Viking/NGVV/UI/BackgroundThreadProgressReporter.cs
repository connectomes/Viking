﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Viking.UI
{
    public class BackgroundThreadProgressReporter : Viking.Common.IProgressReporter
    {
        BackgroundWorker worker;

        public BackgroundThreadProgressReporter(BackgroundWorker worker)
        {
            this.worker = worker;
        }

        public void ReportProgress(double PercentProgress, string message)
        {
            worker.ReportProgress((int)PercentProgress, message);
        }

        public void TaskComplete()
        {
            worker.ReportProgress(100, "Task complete");
        }
    }
}
