using Microsoft.Win32;
using System.Text;
using Tools.Lib.MenuContext.Win32Struct;

namespace Tools.Lib.MenuContext
{
    internal class FileExtension
    {

        public static string GetExtentionInfo(AssocStr assocStr, string extension)
        {
            uint pcchOut = 0;
            Win32DLL.AssocQueryString(AssocF.Verify, assocStr, extension, null, null, ref pcchOut);
            StringBuilder pszOut = new StringBuilder((int)pcchOut);
            Win32DLL.AssocQueryString(AssocF.Verify, assocStr, extension, null, pszOut, ref pcchOut);
            return pszOut.ToString();
        }

        public static string GetOpenMode(string extension)
        {
            if (string.IsNullOrEmpty(extension)) 
                return null;
            string mode;
            bool CheckMode()
            {
                if (string.IsNullOrWhiteSpace(mode)) return false;
                if (mode.Length > 255) return false;
                if (mode.ToLower().StartsWith(@"applications\")) return false;
                using (RegistryKey root = Registry.ClassesRoot)
                using (RegistryKey key = root.OpenSubKey(mode))
                {
                    return key != null;
                }
            }
            mode = Registry.GetValue($@"{RegistryPath.FileExtsPath}\{extension}\UserChoice", "ProgId", null)?.ToString();
            if (CheckMode())
                return mode;
            mode = Registry.GetValue($@"{RegistryPath.HKLMClasses}\{extension}", "", null)?.ToString();
            if (CheckMode())
                return mode;
            mode = Registry.GetValue($@"{RegistryPath.HKCRClasses}\{extension}", "", null)?.ToString();
            if (CheckMode())
                return mode;
            return null;
        }
    }
}
