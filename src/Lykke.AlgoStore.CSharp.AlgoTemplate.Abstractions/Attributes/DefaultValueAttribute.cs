using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public DefaultValueAttribute(object value)
        {
            Value = value;
        }
    }
}
