using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTools.MoreAttributes;
using System;

public class MoreAttributesTest : MonoBehaviour
{
    [ReadOnly]
    public float readOnlyFloat = 152321;

    [BitMask]
    public TestMaskEnum testBitMask = TestMaskEnum.M0;
}

public enum TestMaskEnum
{
    M0 = 1 << 0,
    M1 = 1 << 1,
    M2 = 1 << 2,
    M3 = 1 << 3,
    M4 = 1 << 4,
    M5 = 1 << 5,
    M6 = 1 << 6,
    M7 = 1 << 7,
    M8 = 1 << 8,
    M9 = 1 << 9,
    M10 = 1 << 10,
    M11 = 1 << 11,
    M12 = 1 << 12,
    M13 = 1 << 13,
    M14 = 1 << 14,
    M15 = 1 << 15,
    M16 = 1 << 16,
    M17 = 1 << 17,
    M18 = 1 << 18,
    M19 = 1 << 19,
    M20 = 1 << 20,
    M21 = 1 << 21,
    M22 = 1 << 22,
    M23 = 1 << 23,
    M24 = 1 << 24,
    M25 = 1 << 25,
    M26 = 1 << 26,
    M27 = 1 << 27,
    M28 = 1 << 28,
    M29 = 1 << 29,
    M30 = 1 << 30,
    M31 = 1 << 31,    
}
