using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.UtilPack;

public class TypeTester : MonoBehaviour
{
    [TypeRestriction(typeof(TestBaseType))]
    public SerializableType typeVar = new SerializableType(typeof(TestChildTypeC));

    [TypeRestriction(typeof(TestBehaviourBase))]
    public SerializableType behaviourToAdd = new SerializableType(typeof(TestBehaviourBase));

    private void Awake()
    {
        ((TestBaseType)typeVar.GetNewInstanceOfType()).TestLog();
        gameObject.AddComponent(behaviourToAdd);
    }
}

public class TestBaseType
{
    public virtual void TestLog()
    {
        Debug.Log("TestBaseType");
    }
}

public class TestChildTypeA : TestBaseType
{
    public override void TestLog()
    {
        Debug.Log("TestChildTypeA");
    }
}

public class TestChildTypeB : TestBaseType
{
    public override void TestLog()
    {
        Debug.Log("TestChildTypeB");
    }
}

public class TestChildTypeC : TestBaseType
{
    public override void TestLog()
    {
        Debug.Log("TestChildTypeC");
    }
}

public class TestChildTypeD : TestBaseType
{
    public override void TestLog()
    {
        Debug.Log("TestChildTypeD");
    }
}
