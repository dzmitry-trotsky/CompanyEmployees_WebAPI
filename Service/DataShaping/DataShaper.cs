using Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Service.DataShaping
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        //all properties of input class
        public PropertyInfo[] Properties { get; set; }

        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            
        }

        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> shapedEntities, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);

            return FetchData(shapedEntities , requiredProperties);
        }

        public ShapedEntity ShapeData(T shapedEntity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);

            return FetchDataForEntity(shapedEntity, requiredProperties);
        }

        //extract the values from the required properties and bind it to ExpandoObject
        private ShapedEntity FetchDataForEntity(object shapedEntity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();

            foreach(var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(shapedEntity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            //id for shaped object adding separately
            var objectProperty = shapedEntity.GetType().GetProperty("Id");
            shapedObject.Id = (Guid) objectProperty.GetValue(shapedEntity);

            return shapedObject;
        }

        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> shapedEntities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();

            foreach(var entity in shapedEntities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }

            return shapedData;
        }

        //parses the input string and returns just the properties we need to return to the controller
        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();

            if(!string.IsNullOrWhiteSpace(fieldsString))
            {
                var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach(var field in fields)
                {
                    var property = Properties.FirstOrDefault(pi => 
                            pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                    if (property == null)
                        continue;

                    requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = Properties.ToList();
            }

            return requiredProperties;
        }
    }
}
