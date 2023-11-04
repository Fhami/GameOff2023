using System.Collections.Generic;

namespace DefaultNamespace
{
    public class PropertyContainer
    {
        public Dictionary<PropertyKey, IProperty> Properties { get; set; }

        public void Add<T>(PropertyKey key, Property<T> property)
        {
            Properties[key] = property;
        }

        public Property<T> Get<T>(PropertyKey key) => Properties[key] as Property<T>;

        public bool Has(PropertyKey key)
        {
            return Properties.ContainsKey(key);
        }

        public void Remove(PropertyKey key)
        {
            Properties.Remove(key);
        }
    }
}