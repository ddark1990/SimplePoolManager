# SimplePoolManager
Pool Manager plugin for unity with a custom inspector.

# Overview: 
This is a simple pool manager that allows you to pool any unity GameObject with ease. Has a custom built inspector for easy use and has the option to expand the pools if they run out.
This tool is free to use with any type of projects weather pro or personal.

# How To Install:
You can download the unity package which contains the scene and the plugin scripts themselves from [here](https://github.com/ddark1990/SimplePoolManager/blob/master/SimplePoolManager-Goomer.unitypackage),
or you can clone the entire project if you wish. Once you have the package open up Unity and go to Assets in the top left corner, then import package, then import custom package.

# How To Use:
In the Prefabs folder there is a prefab called PoolManager which you can just drag into your scene and start using.
Check "Expand Pools If Empty" if you want you're pools to auto expand beyond the designated value.

![](PoolManagerPlugin/PoolManagerPics/pm1.png)

# Interface:
Its interfaced for OnObjectSpawn & OnObjectDespawn with IPooledObject which you can add to any object that you are pooling to get subscribe to those events.
