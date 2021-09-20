using System;
using System.Diagnostics;

namespace PSQuickAssets.PSInterop
{
    public class PhotoshopInterop : IPhotoshopInterop
    {
        private const int ERR_GENERAL_PS_ERROR = -2147212704;
        private const int ERR_RETRY_LATER = -2147417846;
        private const int ERR_INVALID_FILE_FORMAT = -2147213504;
        private const int ERR_ILLEGAL_ARGUMENT = -2147220262;
        private const int ERR_INVALID_PATH = -2147220271;

        /// <summary>
        /// Attempts to add given image (filepath) to open document in PS.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        public PSResult AddImageToDocument(string filePath)
        {
            if (Process.GetProcessesByName("Photoshop").Length == 0)
                return new PSResult(PSStatus.NotRunning, filePath, "Photoshop is not running");

            try
            {
                dynamic ps = CreatePhotoshopCOMInstance();
                PlaceImageInDoc(filePath, ps);
                return new PSResult(PSStatus.Success, filePath, "");
            }
            catch (Exception ex) when (ex.HResult == ERR_GENERAL_PS_ERROR && ex.Message.Contains("The command \"Place\" is not currently available"))
            {
                return new PSResult(PSStatus.NoDocumentsOpen, filePath, "No documents open");
            }
            catch (Exception ex) when (ex.HResult == ERR_GENERAL_PS_ERROR && ex.Message.Contains("no parser or file format can open the file"))
            {
                return new PSResult(PSStatus.InvalidFileFormat, filePath, "Invalid file format");
            }            
            catch (Exception ex) when (ex.HResult == ERR_GENERAL_PS_ERROR && ex.Message.Contains("file could not be found"))
            {
                return new PSResult(PSStatus.FileNotFound, filePath, "File could not be found");
            }
            catch (Exception ex) when (ex.HResult == ERR_RETRY_LATER)
            {
                return new PSResult(PSStatus.Busy, filePath, "Photoshop is busy");
            }
        }

        /// <summary>
        /// Open image as new document.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        /// <exception cref="Exception"></exception>
        public PSResult OpenImage(string filePath)
        {
            if (Process.GetProcessesByName("Photoshop").Length == 0)
                return new PSResult(PSStatus.NotRunning, filePath, "Photoshop is not running");

            try
            {
                dynamic ps = CreatePhotoshopCOMInstance();
                ps.Open(filePath);
                return new PSResult(PSStatus.Success, filePath, "");
            }
            catch (Exception ex) when (ex.HResult == ERR_RETRY_LATER)
            {
                return new PSResult(PSStatus.Busy, filePath, "Photoshop is busy");
            }
            catch (Exception ex) when (ex.HResult == ERR_INVALID_FILE_FORMAT)
            {
                return new PSResult(PSStatus.InvalidFileFormat, filePath, "Invalid file format");
            }
            catch (Exception ex) when (ex.HResult == ERR_ILLEGAL_ARGUMENT)
            {
                return new PSResult(PSStatus.IllegalArgument, filePath, "Filepath is not valid");
            }
            catch (Exception ex) when (ex.HResult == ERR_INVALID_PATH)
            {
                return new PSResult(PSStatus.IllegalArgument, filePath, "Filepath is not valid");
            }
            catch (Exception ex) when (ex.HResult == ERR_GENERAL_PS_ERROR && ex.Message.Contains("file could not be found"))
            {
                return new PSResult(PSStatus.FileNotFound, filePath, "File could not be found");
            }
        }

        private static dynamic CreatePhotoshopCOMInstance()
        {
            return Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.Application"));
        }

        private static PSResult PlaceImageInDoc(string filePath, dynamic ps)
        {
            dynamic actionDescriptor = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));

            if (actionDescriptor is null)
                return new PSResult(PSStatus.COMError, filePath, "Failed to create Action Descriptor");

            dynamic idPlc = ps.CharIDToTypeID("Plc ");
            dynamic idnull = ps.CharIDToTypeID("null");
            actionDescriptor.PutPath(idnull, filePath);
            dynamic idFTcs = ps.CharIDToTypeID("FTcs");
            dynamic idQCSt = ps.CharIDToTypeID("QCSt");
            dynamic idQcsa = ps.CharIDToTypeID("Qcsa");
            actionDescriptor.PutEnumerated(idFTcs, idQCSt, idQcsa);
            ps.ExecuteAction(idPlc, actionDescriptor, PsDialogModes.psDisplayNoDialogs);

            return new PSResult(PSStatus.Success, filePath, "");
        }
    }

    public enum PsDialogModes
    {
        psDisplayAllDialogs = 1,
        psDisplayErrorDialogs = 2,
        psDisplayNoDialogs = 3
    }
}
