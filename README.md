# About
Bulk Tools is my modular tools package. All future tools I make will be made as a module for this package. Any module can be enabled or disabled at any point. 

# Importing into Unity
- Open Package Manager
- Select Add package from git URL
- paste https://github.com/Ryantheinventor/BulkToolsForUnity.git?path=/Assets/BulkTools

# Usage
After importing everything is enabled on all possible platforms by default. To change any configuration of the included modules navigate to the Module Manager window via the BulkTools/Module Manager menu item. 

In the Module Manager you can configure which modules are enabled and on what platforms.
- Enabling a module enables it in the editor:
- To enable it in builds the target platform must be enabled as well.

If an enabled module has any dependencies on other modules, the dependencies will be forced to be enabled on at least the same platforms. Dependencies can not be disabled unless all modules that depend on them are disabled first.

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