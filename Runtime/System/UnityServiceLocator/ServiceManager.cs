using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mimizh.UnityUtilities
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        public IEnumerable<object> RegisteredServices => _services.Values;

        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object serviceObject))
            {
                service = serviceObject as T;
                return true;
            }
            service = null;
            return false;
        }
         
        public T Get<T>() where T : class
        {   
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object service))
            {
                return service as T;
            }
            throw new ArgumentException($"Service type not found: type:{type.FullName}");
        }
        
        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);
            if (!_services.TryAdd(type, service))
            {
                Debug.LogError($"Service:{type.FullName} already registered");
            }
            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException($"服务与类型不匹配 {type.FullName}");
            }
            
            if (!_services.TryAdd(type, service))
            {
                Debug.LogError($"Service:{type.FullName} already registered");
            }
            return this;
        }
    }
}