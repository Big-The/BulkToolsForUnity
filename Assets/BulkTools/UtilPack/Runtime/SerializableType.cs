using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace BTools.UtilPack
{
    //Based on https://www.dropbox.com/scl/fi/uhb9l2dcvfzgxbduqkvao/SerializableType.cs?rlkey=py7n6asrlu94t8y0qt6hh4kcj&e=1&dl=0
    //found in https://discussions.unity.com/t/assign-a-variable-of-type-type-on-the-inspector-or-best-work-around/202952/3
    [System.Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        private Type type = typeof(object);
        public Type SavedType
        {
            get => type;
        }

        public byte[] data;

        public SerializableType(Type type)
        {
            this.type = type;
            data = null;
        }

        public object GetNewInstanceOfType() 
        {
            return Activator.CreateInstance(type);
        }

        public static implicit operator Type(SerializableType self) 
        {
            return self.type;
        }

        public static System.Type Read(BinaryReader aReader)
        {
            var paramCount = aReader.ReadByte();
            if (paramCount == 0xFF)
                return null;
            var typeName = aReader.ReadString();
            var type = System.Type.GetType(typeName);
            if (type == null)
                throw new System.Exception("Can't find type; '" + typeName + "'");
            if (type.IsGenericTypeDefinition && paramCount > 0)
            {
                var p = new System.Type[paramCount];
                for (int i = 0; i < paramCount; i++)
                {
                    p[i] = Read(aReader);
                }
                type = type.MakeGenericType(p);
            }
            return type;
        }


        public static void Write(BinaryWriter aWriter, System.Type aType)
        {
            if (aType == null)
            {
                aWriter.Write((byte)0xFF);
                return;
            }
            if (aType.IsGenericType)
            {
                var t = aType.GetGenericTypeDefinition();
                var p = aType.GetGenericArguments();
                aWriter.Write((byte)p.Length);
                aWriter.Write(t.AssemblyQualifiedName);
                for (int i = 0; i < p.Length; i++)
                {
                    Write(aWriter, p[i]);
                }
                return;
            }
            aWriter.Write((byte)0);
            aWriter.Write(aType.AssemblyQualifiedName);
        }




        public void OnBeforeSerialize()
        {
            data = TypeToBytes(type);
        }


        public void OnAfterDeserialize()
        {
            type = BytesToType(data);
        }

        public static Type BytesToType(byte[] bytes) 
        {
            using (var stream = new MemoryStream(bytes))
            using (var r = new BinaryReader(stream))
            {
                return Read(r);
            }
        }

        public static byte[] TypeToBytes(Type type) 
        {
            using (var stream = new MemoryStream())
            using (var w = new BinaryWriter(stream))
            {
                Write(w, type);
                return stream.ToArray();
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class TypeRestrictionAttribute : Attribute 
{
    public Type type;
    public TypeRestrictionAttribute(Type type) 
    {
        this.type = type;
    }
}