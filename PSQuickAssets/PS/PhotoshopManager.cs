using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Photoshop;
using PSQuickAssets.PS;
using PSQuickAssets.Utils;

namespace PSQuickAssets
{
    public record PSResult(PSCallResult CallResult, string FilePath, string Message);

    public enum PSCallResult
    {
        Success,
        NotRunning,
        NoDocumentsOpen,
        Busy,
        Failed
    }

    public class PhotoshopManager
    {
        private const int ERR_NO_SUCH_ELEMENT = -2147212704;
        private const int ERR_RETRY_LATER = -2147417846;

        public async Task<PSResult> AddImageToDocAsync(string filePath)
        {
            return await Task.Run(() => AddImageToDoc(filePath));
        }

        /// <summary>
        /// Attempts to add given image (filepath) to open document in PS.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns><see langword="true"/> if placed successfully.</returns>
        public PSResult AddImageToDoc(string filePath)
        {
            if (WindowControl.IsProcessRunning("Photoshop") == false)
                return new PSResult(PSCallResult.NotRunning, filePath, "Photoshop is not running");

            Debug.WriteLine("Ps is running");

            try
            {
                dynamic ps = CreatePhotoshopCOMInstance();
                Debug.WriteLine("Created PS instance.");
                PlaceImage(filePath, ps);
                return new PSResult(PSCallResult.Success, filePath, "");
            }
            catch (Exception ex) when (ex.HResult == ERR_NO_SUCH_ELEMENT)
            {
                Debug.WriteLine("No documents open.");
                return new PSResult(PSCallResult.NoDocumentsOpen, filePath, "No documents open");
            }
            catch (Exception ex) when (ex.HResult == ERR_RETRY_LATER)
            {
                Debug.WriteLine("Photoshop is busy.");
                return new PSResult(PSCallResult.Busy, filePath, "Photoshop is busy");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message);
                return new PSResult(PSCallResult.Failed, filePath, ex.Message);
            }
        }

        private static dynamic CreatePhotoshopCOMInstance()
        {
            //var guid = new Guid("{0e9aaf8c-058b-433d-a42b-5b98325fa81c}");
            //var guid = new Guid("{e891ee9a-d0ae-4cb4-8871-f92c0109f18e}");

            //var type = Type.GetTypeFromProgID("Photoshop.Application");
            //var typeOld = Type.GetTypeFromCLSID(guid);

            //dynamic instance = Activator.CreateInstance(typeOld);

            //return Activator.CreateInstance(Type.GetTypeFromCLSID(guid));
            //return COMMarshal.GetActiveObject("Photoshop.Application");
            //var instance = Activator.CreateInstance(Type.get)
            return Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.Application"));
        }

        private static void PlaceImage(string filePath, dynamic ps)
        {
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
    }
}
