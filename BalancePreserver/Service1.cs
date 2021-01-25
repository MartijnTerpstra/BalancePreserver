using NAudio.CoreAudioApi;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace BalancePreserver
{
    public partial class Service1 : ServiceBase
    {
        static Task m_task;
        static readonly ManualResetEvent m_quitEvent = new ManualResetEvent(false);

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            m_task = Task.Run(new Action(HandleAudio));
        }

        protected override void OnStop()
        {
            m_quitEvent.Set();
            m_task.Wait();

            m_quitEvent.Reset();
            m_task = null;
        }

        private static void HandleAudio()
        {
            try
            {
                while (!m_quitEvent.WaitOne(60000))
                {
                    try
                    {
                        using (MMDeviceEnumerator devEnum = new MMDeviceEnumerator())
                        {
                            using (MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console))
                            {
                                var channels = defaultDevice.AudioEndpointVolume.Channels;

                                if (channels.Count == 2)
                                {
                                    if (channels[0].VolumeLevelScalar > channels[1].VolumeLevelScalar)
                                    {
                                        channels[1].VolumeLevelScalar = channels[0].VolumeLevelScalar;
                                    }
                                    else if (channels[0].VolumeLevelScalar < channels[1].VolumeLevelScalar)
                                    {
                                        channels[0].VolumeLevelScalar = channels[1].VolumeLevelScalar;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }
    }
}
