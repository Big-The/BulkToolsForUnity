# Contents
- [About The Package](#about-the-package)
- [About The Modules](#about-the-modules)
- [Importing Into Unity](#importing-into-unity)
- [Pacakge Usage](#package-usage)
- [Module Usage](#module-usage)
- [Build Targets](#generally-supported-build-targets)
- [Version Goals](#version-goals)

# About The Package
Bulk Tools is my modular tools package. All future tools I make will be made as a module for this package. Any module can be enabled or disabled at any point. 

## About The Modules:
- Asset Replacement

# Importing Into Unity
- Open Package Manager
- Select Add package from git URL
- paste https://github.com/Ryantheinventor/BulkToolsForUnity.git?path=/Assets/BulkTools

# Package Usage
After importing everything is enabled on all possible platforms by default. To change any configuration of the included modules navigate to the Module Manager window via the BulkTools/Module Manager menu item. 

In the Module Manager you can configure which modules are enabled and on what platforms.
- Enabling a module enables it in the editor:
- To enable it in builds the target platform must be enabled as well.

If an enabled module has any dependencies on other modules, the dependencies will be forced to be enabled on at least the same platforms. Dependencies can not be disabled unless all modules that depend on them are disabled first.

# Module Usage
Individual modules include thier own READMEs with specific usage info. 

Pre [1.0.0] Note: Not all packages will have READMEs yet. (If you see this note and the current version is past [1.0.0] please submit an issue about it)

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

# Version Goals

## [0.1.0] First Publicaly Avalible Version
- Most functionality required to make the package easily useable by people other than me is present
- Ability to add "settings" in the form of defines that can be toggled in the editor window

## [0.2.0] External Dependencies Support
- Support for modules that have dependencies outside of the other modules. For example: depends on TMPro
  - Automatically add required external dependencies

## All Versions In-between
- Working to get modules out of experimental

## [1.0.0] First Major Version
- All early modules out of experimental phase
- Minimal chances of breaking