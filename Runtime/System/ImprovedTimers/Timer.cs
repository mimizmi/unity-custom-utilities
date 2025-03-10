﻿using UnityEngine;
using System;

namespace Mimizh.UnityUtilities
{
    public abstract class Timer : IDisposable
    {
        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }

        protected float initialTime;
        
        public float Progress => Mathf.Clamp01(CurrentTime / initialTime);
        
        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
        }

        public void Start()
        {
            CurrentTime = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }
        public abstract void Tick();
        public abstract bool IsFinished { get; }
        
        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;
        
        public virtual void Reset() => CurrentTime = initialTime;

        public virtual void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
        bool _disposed;

        ~Timer()
        {
            Dispose(false);
        }
        
        // Call Dispose to ensure deregistration of the timer from the timermanager
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }
            _disposed = true;
        }
    }
}