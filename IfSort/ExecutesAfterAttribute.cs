using System;

namespace IfSort
{
    public class ExecutesAfterAttribute : Attribute
    {
        readonly Type type;

        public ExecutesAfterAttribute(Type type)
        {
            this.type = type;
        }

        public Type Type
        {
            get { return type; }
        }
    }
}