using System.Reflection;

namespace PRISM.DTO
{
    public class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        public static readonly PropertyInfoComparer Instance = new PropertyInfoComparer();

        public bool Equals(PropertyInfo x, PropertyInfo y)
        {
            return x.Name == y.Name && x.PropertyType == y.PropertyType;
        }

        public int GetHashCode(PropertyInfo obj)
        {
            return obj.Name.GetHashCode() ^ obj.PropertyType.GetHashCode();
        }
    }
   
}
