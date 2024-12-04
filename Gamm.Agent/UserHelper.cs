using System;
using System.Management;

namespace Gamm.Agent
{
    public class UserHelper
    {
        public static string GetLoggedInUserName()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT UserName FROM Win32_ComputerSystem"))
            {
                foreach (var queryObj in searcher.Get())
                {
                    var username = queryObj["UserName"];
                    if (username != null)
                    {
                        return username.ToString();
                    }
                }
            }
            return Environment.UserName;
        }
    }
}
