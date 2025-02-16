using System;

namespace Mimizh.UnityUtilities.DisposableUtils
{
    
    public class DisposableSetBoolean : IDisposable
    {
        private readonly Action<bool> _action;
        private readonly bool _startingValue;

        /// <summary>
        /// Set an action auto invoke bool when new & dispose
        /// </summary>
        /// <param name="action">action with bool</param>
        /// <param name="startingValue">start value</param>
        public DisposableSetBoolean(Action<bool> action, bool startingValue)
        {
            _action = action;
            _startingValue = startingValue;
            _action?.Invoke(startingValue);
        }

        public void Dispose()
        {
            _action?.Invoke(!_startingValue);
        }
    }
}