using System;
using UnityEngine;
namespace Mimizh.UnityUtilities
{
    public class StatModifier : IDisposable {
        public StatType Type { get; }
        public IOperationStrategy Strategy { get; }
    
        public readonly Sprite icon;
        public bool MarkedForRemoval { get; set; } // TODO: Make private and add a public method to set it
    
        public event Action<StatModifier> OnDispose = delegate { };
    
        readonly CountDownTimer timer;

        public StatModifier(StatType type, IOperationStrategy strategy, float duration) {
            Type = type;
            Strategy = strategy;
            if (duration <= 0) return;
        
            timer = new CountDownTimer(duration);
            timer.OnTimerStop += () => MarkedForRemoval = true;
            timer.Start();
        }
    
        public void Handle(object sender, Query query) {
            if (query.StatType == Type) {
                query.Value = Strategy.Calculate(query.Value);
            }
        }

        public void Dispose() {
            OnDispose.Invoke(this);
            timer.Dispose();
        }
    }
}