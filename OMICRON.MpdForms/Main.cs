using mtronix;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace OMICRON.MpdForms
{
    public partial class Main : Form, IConsumer2
    {
        readonly double qIecHandle;
        readonly double vRmsHandle;
        public Dispatcher Dispatcher { get; set; }
        public Main()
        {
            InitializeComponent();

            var app = new mtronixMeasurementApp();
            qIecHandle = app.registerConsumer2("station1.qIec", "MPD540", this);
            vRmsHandle = app.registerConsumer2("station1.vRms", "MPD540", this);
            Dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void notifyValueEvent(int handle, object v)
        {
            
            var value = (double)v;
            if (handle == qIecHandle)
            {
                Dispatcher.Invoke(new Action(() => {
                    textBoxQiec.Text = (value * 1000000000000.000000).ToString();
                }), null);
            } 
            if(handle == vRmsHandle)
            {
                Dispatcher.Invoke(new Action(() => {
                    textBoxVrms.Text = (value * 1000.000000).ToString();
                }), null);
            }
        }
    }
}
