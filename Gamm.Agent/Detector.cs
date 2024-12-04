using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Management;
using System.Linq;

namespace Gamm.Agent
{
    internal static class Detector
    {
        // MotherBoard
        public static IEnumerable<Models.AgentPartModel> GetMotherBoards()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "Main Board" };
                    if (mo["Product"] != null) model.Model = mo["Product"].ToString();
                    if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                    if (mo["Description"] != null) model.Comment = mo["Description"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MotherBoard: " + ex.ToString());
            }
            return list;
        }

        // Processor
        public static IEnumerable<Models.AgentPartModel> GetProcessors()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM  Win32_Processor");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "Cpu" };
                    var name = "";
                    if (mo["Name"] != null) name = mo["Name"].ToString();
                    if (name.Contains("Pentium")) name = name.Replace("Xeon processor", "").Trim();
                    if (mo["SocketDesignation"] != null) name += " - " + mo["SocketDesignation"].ToString();
                    model.Model = name;

                    if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                    if (mo["Description"] != null) model.Comment = mo["Description"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Processor: " + ex.ToString()); }
            return list;
        }


        // DiskDrive
        public static IEnumerable<Models.AgentPartModel> GetDisks()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DiskDrive");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "Hard" };
                    string interfaceType = "";
                    if (mo["InterfaceType"] != null) interfaceType = mo["InterfaceType"].ToString();
                    if (interfaceType == "USB") model.Part = "Hard USB";

                    var name = "";
                    if (mo["Model"] != null) name = mo["Model"].ToString();
                    string size = "";
                    try
                    {
                        if (mo["Size"] != null) size = " (" + Int64.Parse(mo["Size"].ToString()) / 1024 / 1024 / 1024 + " GB)";
                    }
                    catch { }
                    model.Model = name + size;

                    if (mo["Caption"] != null) model.Brand = mo["Caption"].ToString();
                    if (mo["Description"] != null) model.Comment = mo["Description"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Disk: " + ex.ToString()); }
            return list;
        }

        // Video
        /*public static IEnumerable<Models.AgentPartModel> GetVideos()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DisplayConfiguration");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "VGA" };

                    string deviceName = mo["DeviceName"]?.ToString() ?? string.Empty;
                    string description = mo["Description"]?.ToString() ?? string.Empty;

                    if (deviceName.Contains("VMware") || description.Contains("Virtual") ||
                        deviceName.Contains("Remote") || description.Contains("Microsoft Remote Display"))
                    {
                        continue; 
                    }

                    model.Model = deviceName;
                    model.Brand = model.Model;
                    model.Comment = description;

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Video: " + ex.ToString()); }
            return list;
        }*/



        public static IEnumerable<Models.AgentPartModel> GetVideos()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "VGA" };

                    string deviceName = mo["Name"]?.ToString() ?? string.Empty;
                    string description = mo["Description"]?.ToString() ?? string.Empty;

                    if (deviceName.Contains("VMware") ||
                        description.Contains("Virtual") ||
                        deviceName.Contains("DameWare") ||
                        description.Contains("DameWare") ||
                        deviceName.Contains("VMware") ||
                        deviceName.Contains("Remote") ||
                        description.Contains("Microsoft Remote Display"))
                    {
                        continue;
                    }

                    model.Model = deviceName;
                    model.Brand = description; // می‌توانید برند را از توصیف بگیرید یا از فیلد دیگری
                    model.Comment = description;

                    list.Add(model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Video: " + ex.ToString());
            }
            return list;
        }

        // CDRom
        public static IEnumerable<Models.AgentPartModel> GetCDRoms()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_CDROMDrive");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "CD-Rom" };

                    string caption = mo["Caption"]?.ToString() ?? string.Empty;
                    string name = mo["Name"]?.ToString() ?? string.Empty;

                    // Filter out virtual CD-ROM drives based on keywords
                    if (caption.Contains("VMware") || caption.Contains("Virtual") || caption.Contains("Remote") ||
                        name.Contains("VMware") || name.Contains("Virtual") || name.Contains("Remote"))
                    {
                        continue; // Skip virtual devices
                    }

                    model.Model = !string.IsNullOrEmpty(caption) ? caption : name;

                    if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                    if (mo["MediaType"] != null) model.Comment = mo["MediaType"].ToString();

                    list.Add(model);
                }

            }
            catch (Exception ex) { Console.WriteLine("CDRom: " + ex.ToString()); }
            return list;
        }

        // Memory
        public static List<Models.AgentPartModel> GetMemories()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "Ram" };

                    string typeRam = string.Empty;
                    string speed = "";
                    try
                    {
                        if (mo["Speed"] != null) speed = mo["Speed"].ToString();
                    }
                    catch
                    {
                    }

                    int ramType = -1;
                    var str = mo["MemoryType"];
                    if (str != null)
                    {
                        try
                        {
                            ramType = int.Parse(str.ToString());
                        }
                        catch
                        {
                        }
                    }

                    if (ramType > 0)
                    {
                        switch (ramType)
                        {
                            case 1:
                                typeRam = "DDR-3";
                                break;
                            case 20:
                                typeRam = "DDR";
                                break;
                            case 21:
                            case 17:
                                typeRam = "DDR-2";
                                break;
                            default:
                                if (ramType > 22)
                                    typeRam = "DDR-3";
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(typeRam))
                    {
                        try
                        {
                            str = mo["SMBIOSMemoryType"];
                            if (str != null)
                            {
                                ramType = int.Parse(str.ToString());
                                switch (ramType)
                                {
                                    case 1:
                                    case 2:
                                        typeRam = "DDR-3";
                                        break;
                                }
                            }
                        }
                        catch { }
                    }

                    if (string.IsNullOrEmpty(typeRam))
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(speed))
                            {
                                var speedInt = int.Parse(speed);
                                if (speedInt >= 100 && speedInt <= 166)
                                {
                                    typeRam = "SDRAM";
                                }
                                else if (speedInt >= 266 && speedInt <= 400)
                                {
                                    typeRam = "DDR";
                                }
                                else if (speedInt >= 533 && speedInt <= 800)
                                {
                                    typeRam = "DDR-2";
                                }
                                else if (speedInt >= 1066 && speedInt <= 1600)
                                {
                                    typeRam = "DDR-3";
                                }
                                else if (speedInt >= 2133 && speedInt <= 3200)
                                {
                                    typeRam = "DDR-4";
                                }
                            }
                        }
                        catch
                        {
                        }
                    }

                    if (string.IsNullOrEmpty(typeRam)) typeRam = "Unknown";

                    model.Model = Int64.Parse(mo["Capacity"].ToString()) / 1024 / 1024 + "MB " + typeRam + " " + speed;
                    if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                    if (mo["Caption"] != null) model.Comment = mo["Caption"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Memory: " + ex.ToString()); }
            return list;
        }

        // Os
        public static List<Models.AgentPartModel> GetOs()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "Os" };

                    var name = "";
                    if (mo["Caption"] != null) name = mo["Caption"].ToString();
                    if (mo["OSArchitecture"] != null) name += " " + mo["OSArchitecture"].ToString();
                    model.Model = name;

                    if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                    if (mo["Version"] != null) model.Comment = "Os Version= " + mo["Version"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Os: " + ex.ToString()); }
            return list;
        }

        // Network
        public static List<Models.AgentPartModel> GetNetworks()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher adapterQuery = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter = True");

                HashSet<string> networkCardNames = new HashSet<string>(); // برای جلوگیری از تکراری بودن

                foreach (ManagementObject mo in adapterQuery.Get())
                {
                    string name = mo["Name"]?.ToString() ?? string.Empty;
                    string description = mo["Description"]?.ToString() ?? string.Empty;

                    // فیلتر کردن کارت‌های مجازی
                    if (name.Contains("Virtual") ||
                        description.Contains("Virtual") ||
                        name.Contains("Hyper-V") ||
                        description.Contains("Hyper-V") ||
                        name.Contains("VMware") ||
                        description.Contains("VMware") ||
                        name.Contains("Bluetooth") || // نادیده گرفتن دستگاه‌های بلوتوث
                        description.Contains("Bluetooth"))
                    {
                        continue;
                    }

                    // بررسی وجود آدرس MAC
                    if (mo["MACAddress"] != null)
                    {
                        // ایجاد مدل برای کارت شبکه
                        var model = new Models.AgentPartModel { Part = "Lan Card" };

                        if (mo["Name"] != null) model.Model = mo["Name"].ToString();
                        if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                        model.Comment = "Mac: " + mo["MACAddress"].ToString();

                        // فقط کارت شبکه واقعی را اضافه کنید
                        if (!networkCardNames.Contains(name))
                        {
                            list.Add(model);
                            networkCardNames.Add(name); // نام کارت شبکه را به مجموعه اضافه کنید
                        }
                    }
                }

            }
            catch (Exception ex) { Console.WriteLine("Network: " + ex.ToString()); }
            return list;
        }

        // Keyboard
        public static List<Models.AgentPartModel> GetKeyboards()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Keyboard");
                foreach (ManagementObject mo in query.Get())
                {
                    // Check if the keyboard is virtual by looking for common virtual keywords
                    string description = mo["Description"]?.ToString() ?? string.Empty;
                    string caption = mo["Caption"]?.ToString() ?? string.Empty;

                    // Add a condition to skip virtual keyboards
                    if (description.Contains("Virtual") ||
                        caption.Contains("Virtual") ||
                        description.Contains("Microsoft") &&
                        caption.Contains("Virtual") || // Additional check for Microsoft virtual keyboards
                        description.Contains("Hyper-V") ||
                        caption.Contains("Hyper-V") ||
                        description.Contains("VMware") ||
                        caption.Contains("VMware"))
                    {
                        continue; // Skip this iteration if it's a virtual keyboard
                    }
                    var model = new Models.AgentPartModel { Part = "Keyboard" };

                    if (mo["Description"] != null) model.Model = mo["Description"].ToString();
                    if (mo["Caption"] != null) model.Comment = mo["Caption"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Keyboard: " + ex.ToString()); }
            return list;
        }

        // Mouse
        public static List<Models.AgentPartModel> GetMouses()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PointingDevice");
                HashSet<string> mouseNames = new HashSet<string>(); // برای ذخیره نام موس‌ها و جلوگیری از تکراری بودن

                foreach (ManagementObject mo in query.Get())
                {
                    var name = "";
                    if (mo["HardwareType"] != null) name = mo["HardwareType"].ToString();

                    // بررسی اینکه آیا دستگاه یک موس مجازی است
                    if (name.Contains("Virtual") ||
                        (name.Contains("Microsoft") && name.Contains("Virtual")) || // بررسی اضافی برای موس‌های مجازی مایکروسافت
                        (mo["Description"] != null && mo["Description"].ToString().Contains("Virtual")) ||
                        (mo["Caption"] != null && mo["Caption"].ToString().Contains("Virtual")))
                    {
                        continue; // این تکرار را رد کن اگر موس مجازی باشد
                    }

                    // فقط موس‌های HID-compliant یا دیگر موس‌های فیزیکی را اضافه کن
                    if (name == "HID-compliant mouse")
                    {
                        // بررسی اینکه آیا نام موس قبلاً اضافه شده است یا نه
                        if (!mouseNames.Contains(name))
                        {
                            var model = new Models.AgentPartModel { Part = "Mouse" };
                            model.Model = name;
                            if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                            if (mo["Caption"] != null) model.Comment = mo["Caption"].ToString();

                            list.Add(model);
                            mouseNames.Add(name); // نام موس را به مجموعه اضافه کن
                        }
                    }
                }

            }
            catch (Exception ex) { Console.WriteLine("Mouse: " + ex.ToString()); }
            return list;
        }

        // Printer
        public static List<Models.AgentPartModel> GetPrinters()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                //ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Printer WHERE PrintProcessor != 'winprint'");
                //using (var query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Printer WHERE Default = True AND PrintProcessor != 'winprint'"))
                //{
                //    if (query.Get().Count > 0)
                //    {
                //        foreach (ManagementObject mo in query.Get())
                //        {
                //            var portName = "";
                //            if (mo["PortName"] != null) portName = mo["PortName"].ToString().Trim();
                //            if (portName != ":")
                //            {
                //                var model = new Models.AgentPartModel { Part = "Printer" };

                //                if (mo["Name"] != null) model.Model = mo["Name"].ToString();
                //                //if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                //                if (mo["Caption"] != null) model.Comment = mo["Caption"].ToString();

                //                list.Add(model);
                //            }
                //        }
                //    }
                //}


                using (var query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Printer WHERE Default = True"))
                {
                    foreach (ManagementObject mo in query.Get())
                    {
                        var portName = (string)mo["PortName"] ?? "";
                        var printerName = (string)mo["Name"] ?? "";
                        var printerCaption = (string)mo["Caption"] ?? "";



                        // Check if the printer is virtual by looking for common virtual keywords
                        if (portName != ":" &&
                            !printerName.Contains("Virtual") &&
                            !printerCaption.Contains("Virtual") &&
                            !printerName.Contains("Microsoft") &&
                            !printerCaption.Contains("Microsoft") &&
                            !printerName.Contains("Fax") &&
                            !printerCaption.Contains("Fax") &&
                            !printerName.Contains("OneNote") &&
                            !printerCaption.Contains("OneNote") &&
                            !printerName.Contains("PDF") && // Common for virtual PDF printers
                            !printerCaption.Contains("PDF"))
                        {
                            list.Add(new Models.AgentPartModel
                            {
                                Part = "Printer",
                                Model = printerName,
                                Comment = printerCaption
                            });
                        }
                    }

                }
            }
            catch (Exception ex) { Console.WriteLine("Printer: " + ex.ToString()); }
            return list;
        }



        // Monitor
        public static List<Models.AgentPartModel> GetMonitors()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_DesktopMonitor");
                var monitors = query.Get();
                int monitorCount = monitors.Count;

                if (monitorCount > 0)
                {
                    foreach (ManagementObject mo in monitors)
                    {
                        var model = new Models.AgentPartModel { Part = "Monitor" };

                        var name = "";
                        if (mo["Name"] != null) name = mo["Name"].ToString();
                        else if (mo["MonitorType"] != null) name = mo["MonitorType"].ToString();

                        // Check if the monitor is virtual by looking for common virtual keywords
                        if (name.Contains("Virtual") ||
                            name.Contains("Microsoft") ||
                            name.Contains("Hyper-V") ||
                            name.Contains("VMware") ||
                            (mo["MonitorManufacturer"] != null &&
                             mo["MonitorManufacturer"].ToString().Contains("Virtual")))
                        {
                            continue; // Skip this iteration if it's a virtual monitor
                        }

                        // If there is more than one monitor, skip generic monitors
                        if (monitorCount > 1 && (name.Contains("Generic PnP Monitor") || name.Contains("Generic Non-PnP Monitor")))
                        {
                            continue; // Skip generic monitors if there are multiple monitors
                        }

                        if (mo["ScreenWidth"] != null) name += " " + mo["ScreenWidth"].ToString();
                        if (mo["ScreenHeight"] != null) name += "*" + mo["ScreenHeight"].ToString();
                        model.Model = name;

                        if (mo["MonitorManufacturer"] != null) model.Brand = mo["MonitorManufacturer"].ToString();

                        // Add DeviceID for more information
                        if (string.IsNullOrEmpty(model.Brand))
                        {
                            if (mo["DeviceID"] != null)
                            {
                                model.Brand = mo["DeviceID"].ToString();
                            }
                        }

                        list.Add(model);
                    }
                }
            }

            catch (Exception ex) { Console.WriteLine("Monitor: " + ex.ToString()); }
            return list;
        }

        // Bios
        public static List<Models.AgentPartModel> GetBioses()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Bios");
                foreach (ManagementObject mo in query.Get())
                {
                    var model = new Models.AgentPartModel { Part = "Bios" };

                    if (mo["Caption"] != null) model.Model = mo["Caption"].ToString();
                    if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                    if (mo["Description"] != null) model.Comment = mo["Description"].ToString();

                    list.Add(model);
                }
            }
            catch (Exception ex) { Console.WriteLine("Bios: " + ex.ToString()); }
            return list;
        }

        // Sound Card
        public static List<Models.AgentPartModel> GetSoundCards()
        {
            var list = new List<Models.AgentPartModel>();
            try
            {
                ManagementObjectSearcher query = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_SoundDevice");

                HashSet<string> soundCardNames = new HashSet<string>(); // برای جلوگیری از تکراری بودن

                foreach (ManagementObject mo in query.Get())
                {
                    // بررسی نام و توضیحات کارت صدا
                    string name = mo["Name"]?.ToString() ?? string.Empty;
                    string description = mo["Description"]?.ToString() ?? string.Empty;

                    // بررسی اینکه آیا کارت صدا مجازی است یا مربوط به میکروفن
                    if (name.Contains("Virtual") ||
                        description.Contains("Virtual") ||
                        name.Contains("Microphone") ||
                        description.Contains("Microphone") ||
                        name.Contains("Intel") && description.Contains("Smart Sound") || // نادیده گرفتن تکنولوژی‌های مربوط به میکروفن
                        name.Contains("Hyper-V") ||
                        description.Contains("Hyper-V") ||
                        name.Contains("VMware") ||
                        description.Contains("VMware"))
                    {
                        continue; // این تکرار را رد کنید اگر کارت صدا مجازی یا مربوط به میکروفن است
                    }

                    // فقط کارت صدای دیفالت را اضافه کنید
                    if (!soundCardNames.Contains(name))
                    {
                        var model = new Models.AgentPartModel { Part = "Sound Card" };

                        if (mo["Name"] != null) model.Model = mo["Name"].ToString();
                        if (mo["Manufacturer"] != null) model.Brand = mo["Manufacturer"].ToString();
                        if (mo["Description"] != null) model.Comment = mo["Description"].ToString();

                        list.Add(model);
                        soundCardNames.Add(name); // نام کارت صدا را به مجموعه اضافه کنید
                    }
                }

            }
            catch (Exception ex) { Console.WriteLine("SoundCard: " + ex.ToString()); }
            return list;
        }

        //*****************************************************************************************************************************************

        public static PhysicalAddress GetMacAddress()
        {
            try
            {
                var list = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var nic in list)
                {
                    Console.WriteLine($"Checking NIC: {nic.Name}, Type: {nic.NetworkInterfaceType}, Status: {nic.OperationalStatus}");

                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && nic.OperationalStatus == OperationalStatus.Up)
                    {
                        var mac = nic.GetPhysicalAddress();
                        var macBytes = mac.GetAddressBytes();

                        if (macBytes != null && macBytes.Length > 0)
                        {
                            return mac; // برگرداندن آدرس MAC
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMacAddress: " + ex.ToString());
            }
            return null; // اگر آدرس MAC پیدا نشد
        }

        public static string GetComputerDescription()
        {
            try
            {
                string key = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\lanmanserver\parameters";
                string computerDescription = (string)Registry.GetValue(key, "srvcomment", null);
                return computerDescription;
            }
            catch (Exception ex)
            {
                Console.WriteLine("::GetComputerDescription::");
                throw ex;
            }
        }

        public static string GetIP()
        {
            try
            {
                // ابتدا تلاش برای دریافت آدرس IP محلی
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // بررسی اینکه آیا این رابط Ethernet است و فعال است
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.OperationalStatus == OperationalStatus.Up)
                    {
                        // دریافت آدرس‌های IP
                        var ipProps = ni.GetIPProperties();
                        foreach (var addr in ipProps.UnicastAddresses)
                        {
                            // بررسی اینکه آیا آدرس IPv4 است و در محدوده محلی است
                            if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                                && !IPAddress.IsLoopback(addr.Address)) // جلوگیری از آدرس لوپ بک
                            {
                                return addr.Address.ToString(); // برگرداندن آدرس IP محلی
                            }
                        }
                    }
                }

                // اگر آدرس IP محلی پیدا نشد، آدرس IP عمومی را دریافت کن
                using (WebClient client = new WebClient())
                {
                    string publicIP = client.DownloadString("https://api.ipify.org");
                    return publicIP.Trim(); // برگرداندن آدرس IP عمومی
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("::GetIP:: Error: " + ex.Message);
                return "Error retrieving IP";
            }
        }


        public static IEnumerable<Models.AgentSoftwareModel> GetInstalledApps()
        {
            try
            {
                var curList = new List<string>();
                var resultList = new List<Models.AgentSoftwareModel>();

                resultList.AddRange(GetApps(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall", curList));
                resultList.AddRange(GetApps(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", curList));

                return resultList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("::GetInstalledApps::");
                throw ex;
            }
        }

        private static IEnumerable<Models.AgentSoftwareModel> GetApps(string regKey, List<string> curList)
        {
            var list = new List<Models.AgentSoftwareModel>();
            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(regKey))
                {
                    if (rk != null)
                    {
                        foreach (string skName in rk.GetSubKeyNames())
                        {
                            using (RegistryKey sk = rk.OpenSubKey(skName))
                            {
                                try
                                {
                                    var ob = sk.GetValue("DisplayName");
                                    var name = "";
                                    if (ob != null) name = ob.ToString();
                                    if (!string.IsNullOrEmpty(name)
                                        && !string.IsNullOrEmpty(name.Trim()))
                                    {
                                        if (!curList.Contains(name))
                                        {
                                            curList.Add(name);
                                            var model = new Models.AgentSoftwareModel();
                                            model.Name = name;

                                            var installDate = sk.GetValue("InstallDate");
                                            model.DateInstalled = installDate == null ? System.DateTime.Now : System.DateTime.Parse(installDate.ToString());

                                            var version = sk.GetValue("DisplayVersion");
                                            if (version == null) version = sk.GetValue("Version");
                                            model.Version = version == null ? "" : version.ToString();

                                            var dsc = sk.GetValue("Comments");
                                            model.Comment = dsc == null ? "" : dsc.ToString();

                                            list.Add(model);
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("::GetInstalledApps::");
                throw ex;
            }
            return list;
        }

        public static Models.AgentMainModel GetMainInformation()
        {
            try
            {
                var model = new Models.AgentMainModel();
                var mac = GetMacAddress();
                model.Ip = GetIP();
                model.ComputerName = Environment.MachineName;
                model.MacAddress = mac == null ? "0000" : mac.ToString();
                model.Comment = GetComputerDescription();
                var userName = UserHelper.GetLoggedInUserName();
                if (userName.Contains('\\')) userName = userName.Split('\\')[1];
                //model.UserName = string.IsNullOrEmpty(userName) ? Environment.UserName : userName;
                model.UserName = userName;
                model.Domain = Environment.UserDomainName;
                return model;
            }
            catch (Exception ex)
            {
                Console.WriteLine("::Computer_Information::");
                throw ex;
            }
        }
    }
}