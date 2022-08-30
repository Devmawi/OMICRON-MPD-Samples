using mtronix;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OMICRON.MpdConsole
{
    internal class OmicronApp: IConsumer2
    {
        private const string MTRONIXDomain = "MTRONIX";
        List<string> handleNames = new List<string>()
        {
           "station1.qIec",
           "persistGo",
           //"currentReplayTime"
        };
        List<int> handles = new List<int>();
        ConcurrentQueue<double> queue = new ConcurrentQueue<double>();

        public string MPD540Domain { get; internal set; } = "MPD540";

        private mtronixMeasurementApp app = new mtronixMeasurementApp();

        public OmicronApp()
        {
            foreach (var item in handleNames)
            {
                handles.Add(app.registerConsumer2(item, MPD540Domain, this));
            }

            //handleNames.Add("replayStream");
            //handles.Add(app.registerConsumer2("replayStream", MTRONIXDomain, this));
        }

        public void GetStatus()
        {
            var softwareVersionHandle = app.getHandle("softwareVersion", MTRONIXDomain);
            var softwareVersion = app.getSTRING(softwareVersionHandle);

            var commandLineHandle = app.getHandle("commandLine", MTRONIXDomain);
            var commandLine = app.getSTRING(commandLineHandle);

            var selectedStationHandle = app.getHandle("selectedStation", MTRONIXDomain);
            var selectedStation = app.getSHORT(selectedStationHandle);
        }

        public bool replayStarted { get; set; } = false;

        public void StartReplay()
        {

            app.setSTRING(app.getHandle("playbackStreamFile", MTRONIXDomain), @"C:\Users\piimawi\Downloads\Job0_8750_0_45_210_45_1_220210_155124.stm");
            app.setBOOL(app.getHandle("replayAsFastAsPossible", MTRONIXDomain), true);
            app.setBOOL(app.getHandle("replayStream", MTRONIXDomain), true);

            Thread.Sleep(5000);           

            app.setSTRING(app.getHandle("xmlExport.folder", MPD540Domain), AppDomain.CurrentDomain.BaseDirectory);
            app.setBOOL(app.getHandle("xmlExport.startExport", MPD540Domain), true);

            Thread.Sleep(3000);

            app.setDOUBLE(app.getHandle("playbackStart", MPD540Domain), 0);
            app.setBOOL(app.getHandle("persistMode", MPD540Domain), true);
            app.setBOOL(app.getHandle("persistGo", MPD540Domain), true);

            app.notifyValueEvent += App_notifyValueEvent;

            replayStarted = true;
            //Timer t = new Timer(GetValues, this, 0, 200);
        }

        private void App_notifyValueEvent(int handle, object v)
        {
            Debug.Print(v.ToString());
        }

        public void GetValues(object state)
        {
            var value = app.getDOUBLE(app.getHandle("station1.qIec", MPD540Domain));
            Debug.Print(value.ToString());
            queue.Enqueue((double)value);
        }

        public void notifyValueEvent(int handle, object v)
        {
            var index = handles.IndexOf(handle);
            if (index != -1)
            {
                var variableName = handleNames[index];
                //Debug.Print($"Variable: {variableName}, Value: {v}");

                if (v.GetType() == typeof(Double))
                {
                    //Debug.Print(v.ToString());
                    queue.Enqueue((double)v);
                }
                    

                if (handles[handleNames.IndexOf("persistGo")] == handle && (bool)v == false && replayStarted)
                {
                    var values = queue.ToArray<double>();
                    app.setBOOL(app.getHandle("xmlExport.startExport", MPD540Domain), false);
                }

            }

        }
    }
}
