using System.Collections.Generic;

namespace DefaultNamespace
{
    public class PropertyContainer
    {
        public Dictionary<PropertyKey, IProperty> Properties { get; set; } = new Dictionary<PropertyKey, IProperty>();

        public void Add<T>(PropertyKey key, Property<T> property)
        {
            Properties[key] = property;
        }

        public Property<T> Get<T>(PropertyKey key) => Properties[key] as Property<T>;

        public IProperty Get(PropertyKey key) => Properties[key];

        public bool TryGet(PropertyKey key, out IProperty property)
        {
            return Properties.TryGetValue(key, out property);
        }
        
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