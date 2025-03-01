# Better Tags Module

## About
Adds a new object tagging system. This new tagging system supports multiple tags per GameObject and tagging of Components.

## Platforms
All

## How to Use
- Adding and removing tags in the editor:
    - Make sure a tag list is present via BulkTools/BTags/New Tag List
    - Once a tag list is present you can open it and pre-define tags
    - To add tags to an object select "Add Tag" in the correct context menu. (GameObject or Component context menus)
    - To remove tags navigate to the ObjectTags component on the GameObject and remove it from there.
- Comparing/Adding/Removing tags in script using provided extension methods:
    - Comparing Single Tags: GameObject.IsTagged, or Component.IsTagged
    - Comparing All Tags: GameObject.GetAssignedTags, or Component.GetAssignedTags
    - Adding Tags: GameObject.AddTag, or Component.AddTag
    - Removing Tags: GameObject.RemoveTag, or Component.RemoveTag
- Finding objects by tags in script:
    - GameObjects: GetFirstGameObjectWithTag, GetAllGameObjectsWithTag
    - Components: GetFirstComponentWithTag, GetAllComponentsWithTag

### Cached Tags
When a tag is set to cache it makes any object tagged with the tag get cached automaticaly into an internal tracking system so when fething that tag the action should be near instant. It is recomended to used cached tags on any tag you expect to be querying often.

## Not Yet Supported
- Moving the tag list asset is not currently supported. It must be present at: Assets/Settings/Resources/TagList.asset