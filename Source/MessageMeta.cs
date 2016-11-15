using System;

namespace Cellnet
{
    public struct MessageMeta
    {
        public readonly static MessageMeta Empty = new MessageMeta(null);

        public Type type;
        public uint id;

        public string name
        {
            get { return type.FullName; }
        }

        public MessageMeta(Type t)
        {
            this.type = t;

            if (t != null)
            {
                this.id = StringHash.Hash(t.FullName);
            }
            else
            {
                this.id = 0;
            }
        }
    }

}