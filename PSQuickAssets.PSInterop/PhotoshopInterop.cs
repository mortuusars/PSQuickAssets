using PSQuickAssets.PSInterop.Internal;
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

        private const string _selectionChannelName = "QuickAssetsMask";

        public PSResult AddImageToDocumentWithMask(string filePath, MaskMode maskMode)
        {
            return ExecuteAction(() =>
            {
                dynamic ps = CreatePSInstance();

                PsActions psActions = new PsActions();
                psActions.SaveSelectionAsChannel(ps, _selectionChannelName);
                psActions.AddFilePathAsLayer(ps, filePath);
                psActions.LoadSelectionFromChannel(ps, _selectionChannelName);
                psActions.ApplyMaskFromSelection(ps, MaskMode.RevealSelection);


                psActions.DeleteChannel(ps, _selectionChannelName);
                psActions.UnlinkMask(ps);
            }, 
            filePath);
        }

        private PSResult ExecuteAction(Action action, string filePath)
        {
            if (Process.GetProcessesByName("Photoshop").Length == 0)
                return new PSResult(PSStatus.NotRunning, filePath, "Photoshop is not running");

            try
            {
                action();
                return new PSResult(PSStatus.Success, filePath, "");
            }
            catch (Exception ex) when (ex.HResult == ERR_GENERAL_PS_ERROR && ex.Message.Contains("The command \"Place\" is not currently available"))
            {
                return new PSResult(PSStatus.NoDocumentsOpen, filePath, "No documents open");
            }
            catch (Exception ex) when (ex.HResult == ERR_GENERAL_PS_ERROR && ex.Message.Contains("The command \"Duplicate\" is not currently available"))
            {
                return new PSResult(PSStatus.NoSelection, filePath, "Document has no selection");
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
        /// Attempts to add given image (filepath) to open document in PS.
        /// </summary>
        /// <param name="filePath">Image filepath.</param>
        public PSResult AddImageToDocument(string filePath)
        {
            if (Process.GetProcessesByName("Photoshop").Length == 0)
                return new PSResult(PSStatus.NotRunning, filePath, "Photoshop is not running");

            try
            {
                dynamic ps = CreatePSInstance();
                PsActions psActions = new PsActions();
                psActions.AddFilePathAsLayer(ps, filePath);
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
                dynamic ps = CreatePSInstance();
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

        private static dynamic CreatePSInstance()
        {
            return Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.Application"));
        }
    }
}
