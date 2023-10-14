<p align="center">
    <a href="#"><img src="https://raw.githubusercontent.com/Deaadman/ModComponentUnityTool/release/Images/TitleCardPatchNotes.png"></a>

---

Welcome to the patch notes for this Unity tool. This document offers a comprehensive history of every update made to this tool, ensuring you always remain informed about the latest features, enhancements, and bug fixes that are implemented. In addition to the current updates, it also shares potential ideas for future enhancements.

Please understand that the ideas and features mentioned under "X.X.X" are not set in stone. They represent possible directions we might take, but there's no guarantee they will be implemented. The development of this tool can be influenced by various factors, including real-life obligations or shifting priorities.

| Versions: |
| - |
| [vX.X.X](#vxxx) |
| [v1.0.0 - Initial Launch](#v100---initial-launch) |

---

## vX.X.X:

>**Note:** A bundle of ideas, with no guarantee of implementation.

- Functionality
	- Automated testing? Can detect any errors with the `.json` files before exporting to reduce time wastage.
- UI
    - Make this an entire tab instead of a button, will give the developer access to more options - and I cleaner UI.
	- These options may include...
		- The output directory of the `.modcomponent` file.
		- Which compression method they would like to use for the file.

---

## v1.0.0 - Initial Launch:

> Released on the **14th of October 2023**.

### Highlights / Key Changes:
- Switches the entire tool to be integrated within the **Unity Editor**.
	- Only tested on **2021.3.16f1**.
- Removes the SharpZipLib dependency.
- Improved logging in the console.
- Compression is defaulted to `Optimized`.

### Added:
- Added `[MenuItem]` to allow the button to display within the `Assets` menu.
- Added `package.json` and `.asmdef` files for the Unity Package.
- Added a namespace to the `ModComponentUnityTool.cs` file.

### Changed / Updated:
- Changed all `Console.WriteLine`'s to `EditorUtility.DisplayDialog` and `Debug.Log`'s.
- Changed the `CompressionLevel` to **Optimal** across the board.
- Changed a lot of naming convensions, removed **Extractor** and changed to **Unity Tool**.
<br></br>
- Updated the restrictions of the folder names which can be compiled from just `auto-mapped`, `blueprints` and `gear-spawns` to also now include `bundle` and `localizations`.

### Removed:
- Removed the [**`SharpZipLib`**](https://github.com/icsharpcode/SharpZipLib/releases) dependency.
- Removed a lot of unused code for file paths.