# Changes:

## [0.1.3]

### Added:
- Added new class based state machine
  - Uses objects instead of methods for states
  - Enforces use of transitions

### Fixed:
- Removed a erroneous using statement from the MagicEvents module


## [0.1.2]

### Added:
- Added simple singleton behaviour to the singlton module

### Fixed:
- Fixed forced singleton allowing new instances to be created during application shutdown/playmode end
- Added missing null check to MagicEvent constructor name parameter
- Enforced readonly nature of eventName in MagicEventContext
- Changed List Shuffler logic.



## [0.1.1]

### Added:
- Added Array Duplicate to Simple Editor Tools module
- Added define symbols for each module when enabled

### Updated
- Switched Simple State Machines to follow the Builder Method pattern
  - Updated StateMachines Module readme with new builder pattern example

### Fixed:
- Removed unneeded AssetDatabase.SaveAssets() call that seemed to be triggering odd internal editor file copy errors
- All foldouts in main management window now toggle on label click
- Fixed Simple State Machine AddState method checking if the state was already present being inverted

### Removed:
- Removed "Coroutines With Results" from UtilPack.
  - Found some issues with the implementation that make the feature impractical. 
  - Skipping normal deprecation steps due to being in pre 1.0.0 version.



## [0.1.0]

### Fixes:
- Fixed some missing and inaccurate descriptions
- Fixed internal assembly system treating all assemblies as editor assemblies preventing any builds, and in some cases basic functionality. 

### Added:
- Added READMEs to all modules
- Added serializable types to the UtilPack module



## [0.0.9]

### Fixes:
- Made magic events queue events when triggered inside another event



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