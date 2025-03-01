# Object Pooling Module

## About
Provides a premade component based solution for object pooling.

## Platforms
All

## How to Use
In the editor: Add the Object Pool component to the designated pool owner and configure it as required.
In code:
- To get an object from the pool use PullFromPool on the ObjectPool 
- To release an object back to the pool call ObjectPool.DestroyOrRepool and pass in the object to be released
