using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cellnet
{


    /// <summary>
    /// 消息类型与id的映射表
    /// </summary>
    public class MessageMetaSet
    {
        

        static Dictionary<uint, MessageMeta> _idmap = new Dictionary<uint, MessageMeta>();
        static Dictionary<Type, MessageMeta> _typemap = new Dictionary<Type, MessageMeta>();
        static Dictionary<string, MessageMeta> _namemap = new Dictionary<string, MessageMeta>();
        static bool _inited = false;
        
        /// <summary>
        /// 扫描一个命名空间下的所有消息, 并以名字注册为消息
        /// </summary>
        /// <param name="NameSpace">命名空间名字</param>
        /// <returns></returns>
        public static void StaticInit(Assembly assembly, string NameSpace)
        {
            if (_inited)
                return;

            _inited = true;

            foreach (Type t in assembly.GetTypes())
            {
                if (t.Namespace == NameSpace && t.IsClass)
                {
                    Register(t);
                }
            }
        }

        /// <summary>
        /// 将消息注册
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        public static MessageMeta Register(Type t)
        {
            if (!GetByType(t).Equals(MessageMeta.Empty))
            {
                throw new Exception("重复的消息ID");
            }

            var mi = new MessageMeta(t);
            mi.id = StringHash.Hash(t.FullName);
            mi.type = t;

            _idmap.Add(mi.id, mi);
            _typemap.Add(t, mi);
            _namemap.Add(mi.name, mi);

            return mi;
        }

        /// <summary>
        /// 根据ID取到消息的类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MessageMeta GetByID(uint id)
        {
            MessageMeta t;
            if (_idmap.TryGetValue(id, out t))
            {
                return t;
            }

            return MessageMeta.Empty;
        }

        /// <summary>
        /// 根据类型取到ID
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MessageMeta GetByType(Type t)
        {
            MessageMeta mi;
            if (_typemap.TryGetValue(t, out mi))
            {
                return mi;
            }

            return MessageMeta.Empty;
        }

        public static MessageMeta GetByName(string name)
        {
            MessageMeta mi;
            if (_namemap.TryGetValue(name, out mi))
            {
                return mi;
            }

            return MessageMeta.Empty;
        }

        public static MessageMeta GetByType<T>()
        {
            return GetByType(typeof(T));
        }


    }

}