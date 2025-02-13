﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Mimizh.UnityUtilities
{
    [Serializable]
    public class InterfaceReference<TInterface, TObject> where TObject : Object where TInterface : class
    {
        [SerializeField, HideInInspector] private TObject underlyingValue;

        public TInterface Value
        {
            get => underlyingValue switch
            {
                null => null,
                TInterface @interface => @interface,
                _ => throw new InvalidOperationException(
                    $"{underlyingValue} needs to be implement interface of type {typeof(TInterface).Name}.")
            };
            set => underlyingValue = value switch
            {
                null => null,
                TObject newValue => newValue,
                _ => throw new ArgumentException(
                    $"{value} needs to be implement interface of type {typeof(TObject).Name}.", string.Empty)
            };
        }

        public TObject UnderlyingValue
        {
            get => underlyingValue;
            set => underlyingValue = value;
        }

        public InterfaceReference() { }

        public InterfaceReference(TObject value) => underlyingValue = value;
        
        public InterfaceReference(TInterface value) => underlyingValue = value as TObject;
    }
    
    [Serializable]
    public class InterfaceReference<TInterface> : InterfaceReference<TInterface, Object> where TInterface : class {}
}