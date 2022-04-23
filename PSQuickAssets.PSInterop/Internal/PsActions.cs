using System;

namespace PSQuickAssets.PSInterop.Internal;

/// <summary>
/// Describes the exception that can happen when calling photoshop related functionality.
/// </summary>
public class PhotoshopInteropException : Exception
{
    public PhotoshopInteropException() { }
    public PhotoshopInteropException(string? message) : base(message) { }
    public PhotoshopInteropException(string? message, Exception? innerException) : base(message, innerException) { }
}

internal static class PsActions
{
    private static readonly Type _actionDescriptorType = Type.GetTypeFromProgID("Photoshop.ActionDescriptor")
        ?? throw new PhotoshopInteropException("Registry does not have a type associated with ProgID 'Photoshop.ActionDescriptor'. " +
            "Reinstalling Photoshop might fix the problem.");

    private static readonly Type _actionReferenceType = Type.GetTypeFromProgID("Photoshop.ActionReference")
        ?? throw new PhotoshopInteropException("Registry does not have a type associated with ProgID 'Photoshop.ActionReference'. " +
            "Reinstalling Photoshop might fix the problem.");

    /// <summary>
    /// Adds specified filePath as a new layer.
    /// </summary>
    /// <param name="ps"></param>
    /// <param name="filePath"></param>
    /// <exception cref="Exception"></exception>
    internal static void AddFilePathAsLayer(dynamic ps, string filePath)
    {
        dynamic actionDescriptor = Activator.CreateInstance(_actionDescriptorType);

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
        dynamic desc27 = Activator.CreateInstance(_actionDescriptorType);
        var idnull = ps.CharIDToTypeID("null");
        dynamic ref11 = Activator.CreateInstance(_actionReferenceType);
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
        dynamic desc2 = Activator.CreateInstance(_actionDescriptorType);
        var idNw = ps.CharIDToTypeID("Nw  ");
        var idChnl = ps.CharIDToTypeID("Chnl");
        desc2.PutClass(idNw, idChnl);
        var idAt = ps.CharIDToTypeID("At  ");
        dynamic ref1 = Activator.CreateInstance(_actionReferenceType);
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
        dynamic desc = Activator.CreateInstance(_actionDescriptorType);
        dynamic refer = Activator.CreateInstance(_actionReferenceType);
        refer.PutIdentifier(ps.CharIDToTypeID("Lyr "), ps.ActiveDocument.ActiveLayer.id);
        desc.PutReference(ps.CharIDToTypeID("null"), refer);
        dynamic desc2 = Activator.CreateInstance(_actionDescriptorType);
        desc2.PutBoolean(ps.CharIDToTypeID("Usrs"), false);
        desc.PutObject(ps.CharIDToTypeID("T   "), ps.CharIDToTypeID("Lyr "), desc2);
        ps.ExecuteAction(ps.CharIDToTypeID("setd"), desc, PsDialogModes.psDisplayNoDialogs);
    }
}
