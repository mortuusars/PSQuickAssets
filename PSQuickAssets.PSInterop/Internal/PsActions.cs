using System;

namespace PSQuickAssets.PSInterop.Internal
{
    internal static class PsActions
    {
        /// <summary>
        /// Adds specified filePath as a new layer.
        /// </summary>
        /// <param name="ps"></param>
        /// <param name="filePath"></param>
        /// <exception cref="Exception"></exception>
        internal static void AddFilePathAsLayer(dynamic ps, string filePath)
        {
            dynamic actionDescriptor = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));

            dynamic idPlc = ps.CharIDToTypeID("Plc ");
            dynamic idnull = ps.CharIDToTypeID("null");
            actionDescriptor.PutPath(idnull, filePath);
            dynamic idFTcs = ps.CharIDToTypeID("FTcs");
            dynamic idQCSt = ps.CharIDToTypeID("QCSt");
            dynamic idQcsa = ps.CharIDToTypeID("Qcsa");
            actionDescriptor.PutEnumerated(idFTcs, idQCSt, idQcsa);
            ps.ExecuteAction(idPlc, actionDescriptor, PsDialogModes.psDisplayNoDialogs);
        }

        /// <summary>
        /// Saves current selection to new channel with specified name.
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal static void SaveSelectionAsChannel(dynamic ps, string channelName)
        {
            var idDplc = ps.CharIDToTypeID("Dplc");
            dynamic desc27 = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));
            var idnull = ps.CharIDToTypeID("null");
            dynamic ref11 = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionReference"));
            var idChnl = ps.CharIDToTypeID("Chnl");
            var idfsel = ps.CharIDToTypeID("fsel");
            ref11.PutProperty(idChnl, idfsel);
            desc27.PutReference(idnull, ref11);
            var idNm = ps.CharIDToTypeID("Nm  ");
            desc27.PutString(idNm, channelName);
            ps.ExecuteAction(idDplc, desc27, PsDialogModes.psDisplayNoDialogs);
        }

        /// <summary>
        /// Makes selection from specified channel.
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal static void LoadSelectionFromChannel(dynamic ps, string channelName)
        {
            dynamic channel = ps.ActiveDocument.Channels[channelName];
            ps.ActiveDocument.Selection.Load(channel, PsSelectionType.psReplaceSelection);
        }

        /// <summary>
        /// Adds mask to a current layer.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When mask mode is out of range.</exception>
        /// <exception cref="Exception"></exception>
        internal static void ApplyMaskFromSelection(dynamic ps, MaskMode maskMode)
        {
            string selectionMode = maskMode switch
            {
                MaskMode.RevealAll => "RvlA",
                MaskMode.HideAll => "HdAl",
                MaskMode.RevealSelection => "RvlS",
                MaskMode.HideSelection => "HdSl",
                _ => throw new ArgumentOutOfRangeException(nameof(maskMode), maskMode + " is not supported.")
            };

            var idMk = ps.CharIDToTypeID("Mk  ");
            dynamic desc2 = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));
            var idNw = ps.CharIDToTypeID("Nw  ");
            var idChnl = ps.CharIDToTypeID("Chnl");
            desc2.PutClass(idNw, idChnl);
            var idAt = ps.CharIDToTypeID("At  ");
            dynamic ref1 = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionReference"));
            var idChnl1 = ps.CharIDToTypeID("Chnl");
            var idChnl2 = ps.CharIDToTypeID("Chnl");
            var idMsk = ps.CharIDToTypeID("Msk ");
            ref1.PutEnumerated(idChnl1, idChnl2, idMsk);
            desc2.PutReference(idAt, ref1);
            var idUsng = ps.CharIDToTypeID("Usng");
            var idUsrM = ps.CharIDToTypeID("UsrM");
            var idSelection = ps.CharIDToTypeID(selectionMode);
            desc2.PutEnumerated(idUsng, idUsrM, idSelection);
            ps.ExecuteAction(idMk, desc2, PsDialogModes.psDisplayNoDialogs);
        }

        /// <summary>
        /// Deletes channel with specified name.
        /// </summary>
        /// <exception cref="Exception"></exception>
        internal static void DeleteChannel(dynamic ps, string channelName)
        {
            ps.ActiveDocument.Channels[channelName].Delete();
        }

        /// <summary>
        /// Unlinks mask from layer.
        /// </summary>
        /// <exception cref="Exception"/>
        internal static void UnlinkMask(dynamic ps)
        {
            dynamic desc = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));
            dynamic refer = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionReference"));
            refer.PutIdentifier(ps.CharIDToTypeID("Lyr "), ps.ActiveDocument.ActiveLayer.id);
            desc.PutReference(ps.CharIDToTypeID("null"), refer);
            dynamic desc2 = Activator.CreateInstance(Type.GetTypeFromProgID("Photoshop.ActionDescriptor"));
            desc2.PutBoolean(ps.CharIDToTypeID("Usrs"), false);
            desc.PutObject(ps.CharIDToTypeID("T   "), ps.CharIDToTypeID("Lyr "), desc2);
            ps.ExecuteAction(ps.CharIDToTypeID("setd"), desc, PsDialogModes.psDisplayNoDialogs);
        }        
    }
}
