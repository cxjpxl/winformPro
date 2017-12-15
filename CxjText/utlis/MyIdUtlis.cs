using System;

namespace CxjText.utlis
{
    public class MyIdUtlis
    {
        private static string fingerPrint = "";
        public static string Value()
        {
            if (string.IsNullOrEmpty(fingerPrint))
            {
                fingerPrint = cpuId()  +biosId() + baseId()+videoId() ;
            }
            return fingerPrint;
        }
     
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                
                if (String.IsNullOrEmpty(result))
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("in e");
                    }
                }
            }

            if (String.IsNullOrEmpty(result)) {
                result = "";
            }
            return result;
        }
        private static string cpuId()
        {
          
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") 
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") 
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") 
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                   
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
     
        private static string biosId()
        {
            return identifier("Win32_BIOS", "Manufacturer")
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
            + identifier("Win32_BIOS", "IdentificationCode")
            + identifier("Win32_BIOS", "SerialNumber")
            + identifier("Win32_BIOS", "ReleaseDate")
            + identifier("Win32_BIOS", "Version");
        }
      
        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
       
        private static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }

    }
}
