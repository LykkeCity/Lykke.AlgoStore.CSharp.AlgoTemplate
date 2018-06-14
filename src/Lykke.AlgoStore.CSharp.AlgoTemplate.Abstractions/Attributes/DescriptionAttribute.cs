using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DescriptionAttribute : Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
