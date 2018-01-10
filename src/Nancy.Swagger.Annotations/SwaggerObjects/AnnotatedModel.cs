﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nancy.Swagger.Annotations.Attributes;

namespace Nancy.Swagger.Annotations.SwaggerObjects
{
    public class AnnotatedModel : SwaggerModelData
    {
        public AnnotatedModel(Type type, ModelAttribute modelAttr) : base(type)
        {
            // Only use properties which have a public getter and setter
            var typeProperties = type.GetProperties()
                                        .Where(pi => pi.CanWrite && pi.GetSetMethod(true).IsPublic)
                                        .Where(pi => pi.CanRead && pi.GetGetMethod(true).IsPublic);

            Properties = GetSwaggerModelProperties(typeProperties);
            Description = modelAttr.Description;
        }

        private IList<SwaggerModelPropertyData> GetSwaggerModelProperties(IEnumerable<PropertyInfo> properties)
        {
            var result = new List<SwaggerModelPropertyData>();
            //filter out ignored properties.
            foreach(var prop in properties)
            {
                var attr = prop.GetCustomAttribute<ModelPropertyAttribute>();
                if ((attr != null) && attr.Ignore)
                {
                    continue;
                }
                result.Add(CreateSwaggerModelPropertyData(prop));
            }
            return result;
        }

        private SwaggerModelPropertyData CreateSwaggerModelPropertyData(PropertyInfo pi)
        {
            var modelProperty = new SwaggerModelPropertyData
            {
                Type = pi.PropertyType,
                Name = pi.Name
            };

            foreach (var attr in pi.GetCustomAttributes<ModelPropertyAttribute>())
            {
                modelProperty.Name = attr.Name ?? modelProperty.Name;
                modelProperty.Description = attr.Description ?? modelProperty.Description;
                modelProperty.Minimum = attr.GetNullableMinimum() ?? modelProperty.Minimum;
                modelProperty.Maximum = attr.GetNullableMaximum() ?? modelProperty.Maximum;
                modelProperty.Required = attr.GetNullableRequired() ?? modelProperty.Required;
                modelProperty.UniqueItems = attr.GetNullableUniqueItems() ?? modelProperty.UniqueItems;
                modelProperty.Enum = attr.Enum ?? modelProperty.Enum;
            }

            return modelProperty;
        }
    }
}
