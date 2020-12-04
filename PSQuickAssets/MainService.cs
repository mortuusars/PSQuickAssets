using System;
using System.Diagnostics;
using System.Windows;
using Photoshop;

namespace PSQuickAssets
{
    public static class MainService
    {
        private const int ERR_NO_SUCH_ELEMENT = -2147212704;
        private const int ERR_RETRY_LATER = -2147417846;

        public static void PlaceImage(string filePath)
        {
            if (IsPSRunning() == false)
                return;

            try
            {
                var ps = new ApplicationClass();

                var idPlc = ps.CharIDToTypeID("Plc ");
                var desc3 = new ActionDescriptor();
                var idnull = ps.CharIDToTypeID("null");
                desc3.PutPath(idnull, filePath);
                var idFTcs = ps.CharIDToTypeID("FTcs");
                var idQCSt = ps.CharIDToTypeID("QCSt");
                var idQcsa = ps.CharIDToTypeID("Qcsa");
                desc3.PutEnumerated(idFTcs, idQCSt, idQcsa);
                ps.ExecuteAction(idPlc, desc3, PsDialogModes.psDisplayNoDialogs);
            }
            catch (Exception ex) when (ex.HResult == ERR_NO_SUCH_ELEMENT)
            {
                MessageBox.Show("No documents open.", "PSQuickAssets", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex) when (ex.HResult == ERR_RETRY_LATER)
            {
                MessageBox.Show("Photoshop is busy. Make sure PS is idle and try again.", "PSQuickAssets", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + $"\n\n {filePath}" + "\n\n" + ex.HResult);
            }
        }

        private static bool IsPSRunning()
        {
            var process = Array.Find(Process.GetProcesses(), p => p.ProcessName == "Photoshop");
            return process != null;
        }
    }
}
