using System;
using System.Text;
using System.Windows.Input;

namespace PSQuickAssets.WPF
{
    public class Hotkey
    {
        public Key Key { get; }
        public ModifierKeys Modifiers { get; }

        public Hotkey(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Parse Hotkey from string representation.
        /// </summary>
        /// <param name="hotkey"></param>
        public Hotkey(string hotkey)
        {
            if (hotkey is null)
                throw new ArgumentNullException(nameof(hotkey));

            var keys = hotkey.Split('+');

            var regularKey = keys[^1];
            if (Enum.TryParse<Key>(regularKey, out Key regular) && regular != Key.None)
                Key = regular;
            else
            {
                Key = Key.None;
                Modifiers = ModifierKeys.None;
                return;
            }

            ModifierKeys modifiers = ModifierKeys.None;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (keys[i].Trim() == "Ctrl")
                    modifiers |= ModifierKeys.Control;
                else if (keys[i].Trim() == "Alt")
                    modifiers |= ModifierKeys.Alt;
                else if (keys[i].Trim() == "Shift")
                    modifiers |= ModifierKeys.Shift;
                else if (keys[i].Trim() == "Win")
                    modifiers |= ModifierKeys.Windows;
            }

            Modifiers = modifiers;
        }

        public override string ToString()
        {
            var str = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");

            str.Append(Key);

            return str.ToString();
        }
    }
}
