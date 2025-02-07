﻿using System;

namespace Mimizh.UnityUtilities.SceneManagement
{
    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        private const float ratio = 1f;
        
        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}