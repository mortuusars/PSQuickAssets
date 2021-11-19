using PSQuickAssets.WPF;
using Xunit;
using System.Windows.Input;
using System;
using PSQuickAssets.Services.Hotkeys;

namespace PSQuickAssets.Tests
{
    public class HotkeyParserTests
    {
        [Fact]
        public void HotkeyRegular()
        {
            var expected = new Hotkey(Key.A, ModifierKeys.Control | ModifierKeys.Shift);
            var actual = new Hotkey("Ctrl + Shift + A");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }

        [Fact]
        public void HotkeyHome()
        {
            var expected = new Hotkey(Key.Home, ModifierKeys.Control | ModifierKeys.Shift);
            var actual = new Hotkey("Ctrl + Shift + Home");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }

        [Fact]
        public void HotkeyNum()
        {
            var expected = new Hotkey(Key.NumPad0, ModifierKeys.Control | ModifierKeys.Shift);
            var actual = new Hotkey("Ctrl + Shift + NumPad0");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }

        [Fact]
        public void HotkeyEmpty()
        {
            var expected = new Hotkey(Key.None, ModifierKeys.None);
            var actual = new Hotkey("");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }

        [Fact]
        public void HotkeyNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Hotkey(null));
        }

        [Fact]
        public void HotkeyAsd()
        {
            var expected = new Hotkey(Key.None, ModifierKeys.None);
            var actual = new Hotkey("asd");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }

        [Fact]
        public void HotkeySingle()
        {
            var expected = new Hotkey(Key.A, ModifierKeys.None);
            var actual = new Hotkey("A");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }

        [Fact]
        public void HotkeySingleHome()
        {
            var expected = new Hotkey(Key.Home, ModifierKeys.None);
            var actual = new Hotkey("Home");

            Assert.Equal(expected.Modifiers, actual.Modifiers);
            Assert.Equal(expected.Key, actual.Key);
        }
    }
}
