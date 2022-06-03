### 1.2.0 - 2021-09-20

Added:
- Asset opens as a new document if none are open
- Setting to add a mask to the layer if document had selection prior to adding asset
- Setting to configure if window should be always on top of other windows
- Thumbnail size can be changed with Ctrl+Scroll or Ctrl+Plus/Minus.
- Close button turns red when CTRL is pressed
- Window Show/Hide fading animations
- Beautiful Update window
- If PSQuickAssets crashes it will now create a crash report file that contains some info about an error, instead of ugly message box

Changed:
- Error messages now displayed through Windows notification center
- Default hotkey is now Ctrl + Alt + A
- Slightly tweaked Hotkey Picker: it will now cancel and revert to previous key if ESC is pressed
- App window will be shown on startup instead of splash screen
- Settings now take effect immediatly after changing a setting
- Settings window would be closed when settings button was clicked when it was open

Fixed:
- Tray icon tooltip is now consistent with windows tooltips

Removed:
- Splash screen

---

### 1.1.1 - 2021-09-07

Added:
- Added basic drag and drop. For now only folders are supported. It will add all nested folders too

Fixed:
- Fixed images with small widths displaying too thin.
	- And if one of the sides of an image is too big (in relation to other side) - it will now display fully, without cropping

### 1.1 - 2021-08-02

Added:
- Visual overhaul
- Support for PSD, PSB and Tiff formats
- Window can now be resized and moved
- Settings
- Check if app is already running
- Ctrl + Click on hide button to exit the app

Should now work with all recent Photoshop versions
