using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace Gamm.Agent
{
    class Program
    {
        static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[] { new GammaService() };
            ServiceBase.Run(servicesToRun);
        }

        static void Main_(string[] args)
        {
            Console.WriteLine("Scanning Gamma ITSM Help Desk Data...");
            Console.WriteLine("Please Wait...");
            try
            {
                SetApiToken();

                var model = Detector.GetMainInformation();
                if (ApiHelper.ShouldScan(model))
                {
                    StartScan(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }

        internal static void SetApiToken()
        {
            //ApiHelper.URL = "http://172.16.61.204";
            ApiHelper.URL = "https://crm.kmu.ac.ir";
            ApiHelper.TOKEN = "YbSUfARuKfQz2aneBX0HyV4gODdZdezuwpeoX6lN3ZetcDEuO0Y5Hh3+ryKBNVZn";
        }

        internal static void StartScan(Models.AgentMainModel model)
        {
            ApiHelper.AddAgentMain(model);
            var allParts = GetAllParts(model.Id);
            var allApps = GetAllApps(model.Id);

            ApiHelper.AddAgentPart(allParts);
            ApiHelper.AddAgentSoftware(allApps);

            ApiHelper.SetConflict(model);
        }

        internal static IEnumerable<Models.AgentSoftwareModel> GetAllApps(Guid scanId)
        {
            var listApps = Detector.GetInstalledApps();
            foreach (var item in listApps) { item.ScanId = scanId; }
            return listApps;
        }

        internal static IEnumerable<Models.AgentPartModel> GetAllParts(Guid scanId)
        {
            var listBios = Detector.GetBioses();
            var listOs = Detector.GetOs();
            var listMB = Detector.GetMotherBoards();
            var listCPU = Detector.GetProcessors();
            var listDisk = Detector.GetDisks();
            var listRam = Detector.GetMemories();
            var listVideos = Detector.GetVideos();
            var listCd = Detector.GetCDRoms();
            var listLan = Detector.GetNetworks();
            var listSound = Detector.GetSoundCards();

            var listKeyboard = Detector.GetKeyboards();
            var listMouse = Detector.GetMouses();
            var listPrinter = Detector.GetPrinters();
            var listMonitor = Detector.GetMonitors();

            var partList = listBios
                            .Union(listOs)
                            .Union(listMB)
                            .Union(listCPU)
                            .Union(listDisk)
                            .Union(listRam)
                            .Union(listVideos)
                            .Union(listCd)
                            .Union(listLan)
                            .Union(listSound)
                            .Union(listKeyboard)
                            .Union(listMouse)
                            .Union(listPrinter)
                            .Union(listMonitor);

            foreach (var item in partList) { item.ScanId = scanId; }
            return partList;
        }
    }
}
