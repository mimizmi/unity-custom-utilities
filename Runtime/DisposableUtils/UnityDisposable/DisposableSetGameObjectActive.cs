﻿using System;
using UnityEngine;

namespace Mimizh.UnityUtilities.DisposableUtils
{
    public class DisposableSetGameObjectActive : IDisposable
    {
        private readonly GameObject _gameObject;
        private readonly bool _isStartingActive;

        public DisposableSetGameObjectActive(GameObject gameObject, bool isStartingActive = true)
        {
            _gameObject = gameObject;
            _isStartingActive = isStartingActive;
            _gameObject.SetActive(_isStartingActive);
        }

        public void Dispose()
        {
            _gameObject.SetActive(!_isStartingActive);
        }
    }
}