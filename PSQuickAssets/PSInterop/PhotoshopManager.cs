using System;
using System.Threading.Tasks;
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

    public interface IPhotoshopManager
    {
        PSResult AddImageToDoc(string filePath);
        Task<PSResult> AddImageToDocAsync(string filePath);
    }

    public class PhotoshopManager : IPhotoshopManager
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

            try
            {
                dynamic ps = CreatePhotoshopCOMInstance();
                PlaceImage(filePath, ps);
                return new PSResult(PSCallResult.Success, filePath, "");
            }
            catch (Exception ex) when (ex.HResult == ERR_NO_SUCH_ELEMENT)
            {
                return new PSResult(PSCallResult.NoDocumentsOpen, filePath, "No documents open");
            }
            catch (Exception ex) when (ex.HResult == ERR_RETRY_LATER)
            {
                return new PSResult(PSCallResult.Busy, filePath, "Photoshop is busy");
            }
            catch (Exception ex)
            {
                return new PSResult(PSCallResult.Failed, filePath, ex.Message);
            }
        }

        private static dynamic CreatePhotoshopCOMInstance()
        {
            return Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.Application"));
        }

        private static void PlaceImage(string filepath, dynamic ps)
        {
            var idPlc = ps.CharIDToTypeID("Plc ");
            dynamic descriptor = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));
            var idnull = ps.CharIDToTypeID("null");
            descriptor.PutPath(idnull, filepath);
            var idFTcs = ps.CharIDToTypeID("FTcs");
            var idQCSt = ps.CharIDToTypeID("QCSt");
            var idQcsa = ps.CharIDToTypeID("Qcsa");
            descriptor.PutEnumerated(idFTcs, idQCSt, idQcsa);
            ps.ExecuteAction(idPlc, descriptor, PS.PsDialogModes.psDisplayNoDialogs);
        }
    }
}
