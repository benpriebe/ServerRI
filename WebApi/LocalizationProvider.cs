using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web.Http.Metadata.Providers;
using WebApi.Resx;

namespace WebApi
{
    /// <summary>
    /// Takes the DataAnnotations validation attributes and the DisplayAttribute and localizes them based on a recursive naming structure.
    /// </summary>
    public class LocalizationProvider<TMasterResourceType> : DataAnnotationsModelMetadataProvider
    {
        private readonly ResourceManager _masterResourceManager;

        private static readonly Dictionary<string, Resource> _resourceManagers = new Dictionary<string, Resource>();
        private readonly Type _masterResourceType;

        public LocalizationProvider(ResourceManager masterResourceManager)
        {
            _masterResourceManager = masterResourceManager;
            _masterResourceType = typeof(TMasterResourceType);
        }

        protected override CachedDataAnnotationsModelMetadata CreateMetadataPrototype(
            IEnumerable<Attribute> attributes,
            Type containerType,
            Type modelType,
            string propertyName)
        {
            var metadata = base.CreateMetadataPrototype(attributes, containerType, modelType, propertyName);

            if (containerType == null || propertyName == null)
                return metadata;

            var attrs = attributes.ToList();
            foreach (var attr in attrs)
            {
                var valAttr = attr as ValidationAttribute;
                if (valAttr != null)
                {
                    var key = valAttr.GetType().Name;

                    var strLengthAttr = valAttr as StringLengthAttribute;
                    if (strLengthAttr != null && strLengthAttr.MinimumLength != 0)
                    {
                        key += "_IncludingMinimum";
                    }

                    valAttr.ErrorMessageResourceName = key;
                    valAttr.ErrorMessageResourceType = typeof(ValidationErrors);
                }
                else
                {
                    var attribute = attr as DisplayAttribute; 
                    if (attribute != null && attribute.ResourceType == null)
                    {
                        var resource = GetResource(containerType);
                        if (resource != null)
                        {
                            var resourceKey = GetResourceKey(resource.ResourceManager, containerType, propertyName);
                            if (!String.IsNullOrWhiteSpace(resourceKey))
                            {
                                attribute.Name = resourceKey;
                                attribute.ResourceType = resource.ResourceType;
                            }
                            else
                            {
                                resourceKey = GetMasterResourceKey(propertyName);
                                if (!String.IsNullOrWhiteSpace(resourceKey))
                                {
                                    attribute.Name = resourceKey;
                                    attribute.ResourceType = _masterResourceType;
                                }
                            }
                        }
                    }
                }
            }

            var result = base.CreateMetadataPrototype(attrs, containerType, modelType, propertyName);
            return result;
        }

        public static Resource GetResource(Type containerType)
        {
            var resxClassName = containerType.Namespace + ".Resx.Strings";
            Resource resource;
            _resourceManagers.TryGetValue(resxClassName, out resource);

            if (resource != null)
                return resource;
            
            var assembly = Assembly.GetAssembly(containerType);
            var resourceStrings = assembly.GetType(resxClassName);

            if (resourceStrings == null)
                return null;

            var pi = resourceStrings.GetProperty("ResourceManager", BindingFlags.Public | BindingFlags.Static);
            var rm = (ResourceManager) pi.GetMethod.Invoke(resourceStrings, null);
            resource = new Resource { ResourceManager = rm, ResourceType = resourceStrings };
            _resourceManagers.Add(resxClassName, resource);
            return resource;
        }

        public string GetMasterResourceKey(string propertyName)
        {
            var resourceKey = string.Format("DisplayAttribute_{0}", propertyName);
            var resourceValue = _masterResourceManager.GetString(resourceKey);
            if (!String.IsNullOrWhiteSpace(resourceValue))
                return resourceKey;
            return null;
        }

        /// <summary>
        /// Recursively searches up the tree (base types and interfaces for a resx property value based off the Type passed in.
        /// </summary>
        /// <returns></returns>
        public string GetResourceKey(ResourceManager rm, Type type, string propertyName)
        {
            if (type == null)
                return null;
            
            string resourceKey = string.Format("{0}_DisplayAttribute_{1}", type.Name, propertyName);
            string resourceValue = rm.GetString(resourceKey);

            if (!String.IsNullOrWhiteSpace(resourceValue))  
                return resourceKey;

            resourceKey = GetResourceKey(rm, type.BaseType, propertyName);
            
            if (!String.IsNullOrWhiteSpace(resourceValue))
                return resourceKey;

            // note: no need to check interfaces as [DisplayAttribute] doesn't pass down from them.

            return resourceKey;
        }

        public class Resource
        {
            public ResourceManager ResourceManager { get; set; }
            public Type ResourceType { get; set; }

            public bool Equals(Resource other)
            {
                return Equals(ResourceManager, other.ResourceManager) && Equals(ResourceType, other.ResourceType);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is Resource && Equals((Resource)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ResourceManager != null ? ResourceManager.GetHashCode() : 0) * 397) ^ (ResourceType != null ? ResourceType.GetHashCode() : 0);
                }
            }
        }
    }
}