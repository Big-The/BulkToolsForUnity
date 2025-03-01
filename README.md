# Contents
- [About The Package](#about-the-package)
- [About The Modules](#about-the-modules)
- [Suported Unity Version](#suported-unity-version)
- [Importing Into Unity](#importing-into-unity)
- [Pacakge Usage](#package-usage)
- [Module Usage](#module-usage)
- [Build Targets](#generally-supported-build-targets)
- [Version Goals](#version-goals)

# About The Package
Bulk Tools is my modular tools package. All future generalizable tools I make will be made as a module for this package. Any module can be enabled or disabled at any point. 

## About The Modules:
This package contains the following modules. More information can be found in individual module folders, or by clicking on the module names below:
- [Asset Replacement](Assets/BulkTools/AssetReplacement/AssetReplacement.md)
- [BetterTags](Assets/BulkTools/BetterTags/BetterTags.md)
- [MagicEvents](Assets/BulkTools/MagicEvents/MagicEvents.md)
- [MoreAttributes](Assets/BulkTools/MoreAttributes/MoreAttributes.md)
- [ObjectPooling](Assets/BulkTools/ObjectPooling/ObjectPooling.md)
- [SimpleEditorTools](Assets/BulkTools/SimpleEditorTools/SimpleEditorTools.md)
- [SingletonTools](Assets/BulkTools/SingletonTools/SingletonTools.md)
- [StateMachines](Assets/BulkTools/StateMachines/StateMachines.md)
- [StaticTools](Assets/BulkTools/StateMachines/StateMachines.md)
- [UtilPack](Assets/BulkTools/UtilPack/UtilPack.md)

# Suported Unity Version
Currently targeting 2022.3+

# Importing Into Unity
- Open Package Manager
- Select Add package from git URL
- Paste https://github.com/Big-The/BulkToolsForUnity.git?path=/Assets/BulkTools

# Package Usage
After importing everything is enabled on all possible platforms by default. To change any configuration of the included modules navigate to the Module Manager window via the BulkTools/Module Manager menu item. 

In the Module Manager you can configure which modules are enabled and on what platforms.
- Enabling a module enables it in the editor:
- To enable it in builds the target platform must be enabled as well.

If an enabled module has any dependencies on other modules, the dependencies will be forced to be enabled on at least the same platforms. Dependencies can not be disabled unless all modules that depend on them are disabled first.

# Module Usage
Individual modules include thier own READMEs with specific usage info. Links to these READMEs can be found above in the [About The Modules](#about-the-modules) section.

# Generally Supported Build Targets
Exact supported build targets may vary by module but in general I will be targeting support for the following platform targets:
- Android
- iOS
- LinuxStandalone64
- PS4
- PS5
- Switch
- WebGL
- WindowsStandalone64
- XboxOne

# Version Information

## Change Log
A detailed change log can be found [here](Assets/BulkTools/CHANGELOG.md).

## Version Goals

### [0.2.0] UI Clean Up
- Updates to main window functionality and useability.

### [1.0.0] First Major Version
- All early modules out of experimental phase
    - Minimal chances of breaking

# License Information
This package is provided under the MIT License.
More information about this license can be found [here](Assets/BulkTools/LICENSE).