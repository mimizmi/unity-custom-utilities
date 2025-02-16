using System;
using UnityEngine.UIElements;

namespace Mimizh.UnityUtilities.DisposableUtils
{
    public class DisposableLockButton : IDisposable
    {
        private readonly Button _button;
        private readonly bool _isStartingLocked;

        public DisposableLockButton(Button button , bool isStartingLocked = true)
        {
            _button = button;
            _isStartingLocked = isStartingLocked;
            _button.SetEnabled(!_isStartingLocked);
        }

        public void Dispose()
        {
            _button.SetEnabled(_isStartingLocked);
        }
    }
}