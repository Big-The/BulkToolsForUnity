using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTools.BTags
{
    /// <summary>
    /// The tag class provides a easier way to select tags in the inspector
    /// </summary>
    [System.Serializable]
    public struct Tag
    {
        //tag name value
        public string tagName;

        //The Tag class provides these conversions for easy use when comparing tags
        public static implicit operator string(Tag t) => t.tagName;
        public static explicit operator Tag(string s) => new Tag(){ tagName = s };
    }
}
