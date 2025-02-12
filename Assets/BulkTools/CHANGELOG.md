# Changes:

## [0.0.8]

### Fixes:
- Asset Replacement now takes file type into account correctly
- Magic Events module should now be able to handle null data

## [0.0.7]

### Fixes:
- Magic Events can now no longer have the same callback added to the a single event name multiple times
- Fixed BetterTags add tag popup apearing in only on the primary monitor on multi monitor setups
- Fixed HierarchyTools sort children undo not working

### Added:
- Added global settings support for the modules using symbol defines
- Added external dependency support
- Added GameObjectRandomness to the SimpleEditorTools module
- Added BitMask attribute to More Attributes module

### Updated:
- Magic Events now has a new Editor Window that can be used to inspect what events are being called
  - Includes a define symbol setting that enables more detail but adds some overhead in the editor. Extra detail added:
    - Lists callbacks that respond to each event
    - Extra tab that lists all actively listening callbacks
- Better Tags config can now be accessed in the project settings window
- Changed the SimpleEditorTools class to HierarchyTools as it is not the only class in the module now



## [0.0.6]

### Fixes:
- Module state config is now stored in project settings allowing better support for use in remote repos

### Added:
- Added More Attributes module
  - ReadOnly
- Added Simple Editor Tools module
  - Children Sorting
- Module Config is now avalible in project settings window as well
- Added Asset Replacement Module
  - Used to easily replace asset files without breaking refereneces
- Added Magic Events module
  - Used to connect multiple bits of logic together indirectly

### Updated:
- Changed BTools.Management.Editor namespace to BTools.Management.EditorScritps
- Module Manager logic is now fully static, allowing it to be added to any window easily
- Internal Assembly Definitions are now saved with readable formatting
- Modules are no longer required to have assemblies assigned.



## [0.0.5]

### Updated:
- Updated README to be useful
- Limited platform options to only platforms I plan on actually supporting. Supported platforms are the following
  - Android
  - iOS
  - LinuxStandalone64
  - PS4
  - PS5
  - Switch
  - WebGL
  - WindowsStandalone64
  - XboxOne



## [0.0.4]

### Fixes:
- Swapped remaining RTags naming scheme to new BTags in the Better Tags module

### Added:
- Added inline documentation where it was missing
- Added a second set of apply and revert/refresh buttons to the module manager window

### Updated:
- Changed menu item "BulkTools/Management Window" to "BulkTools/Module Manager" to match the window name better
- Singleton Tools Module now supports singletons outside of the default assembly
- In Random Extras in the Util Pack module the RandomSequence allUnique is now optional and spelled correctly



## [0.0.3]

### Added:
- Added Util Pack Module
    - List Extensions (Shuffling)
    - Random Extras (Sequences, Weighted Random)
    - Coroutines With Results
- Added Object Pool Module

### Updated:
- Updated Experimental tagging on modules that have not undergone sufficient testing



## [0.0.2]

### Fixes:
- Fixed module manager not loading metas in package mode

### Added:
- Changelog file

### Removed:
- Removed Better Tags dependency on TestModule
- Removed TestModule



## [0.0.1]

### Added:
- Added BetterTags module
- Added