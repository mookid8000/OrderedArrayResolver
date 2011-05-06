using System;

namespace IfSort
{
    public class ExecutesBeforeAttribute : Attribute
    {
        readonly Type type;

        public ExecutesBeforeAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }
    }
}