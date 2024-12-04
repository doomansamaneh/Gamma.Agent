using System;
using System.ServiceProcess;
using System.Timers;

namespace Gamm.Agent
{
    partial class GammaService : ServiceBase
    {
        private Timer _timer;
        public GammaService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Scan();
            _timer = new Timer(15 * 60 * 1000);
            _timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Scan();
        }

        private void Scan()
        {
            try
            {
                Program.SetApiToken();

                var model = Detector.GetMainInformation();
                if (ApiHelper.ShouldScan(model))
                {
                    Program.StartScan(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        protected override void OnStop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        }
    }
}