using System;
using UnityEngine;

namespace Mimizh.UnityUtilities.DayNightSystem
{
    public class TimeService
    {
        readonly TimeSettings _settings;
        DateTime _currentTime;
        readonly TimeSpan _sunriseTime;
        readonly TimeSpan _sunsetTime;
        
        public event Action OnSunrise = delegate { };
        public event Action OnSunset = delegate { };
        public event Action OnHourChanged = delegate { };

        private readonly Observable<bool> isDayTime;
        readonly Observable<float> currentHour;

        public TimeService(TimeSettings settings)
        {
            _settings = settings;
            _currentTime = DateTime.Now + TimeSpan.FromHours(settings.startHour);
            _sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
            _sunsetTime = TimeSpan.FromHours(settings.sunsetHour);

            isDayTime = new Observable<bool>(IsDayTime());
            currentHour = new Observable<float>(_currentTime.Hour);
            
            isDayTime.ValueChanged += day => (day ? OnSunrise : OnSunset)?.Invoke();
            currentHour.ValueChanged += _ => OnHourChanged?.Invoke();
        }

        private void UpdateTime(float deltaTime)
        {
            _currentTime = _currentTime.AddSeconds(_settings.timeMultiplier * deltaTime);
            isDayTime.Value = IsDayTime();
            currentHour.Value = _currentTime.Hour;
        }
        
        //base on the day time calculate the sunlight angle
        public float CalculateSunAngle()
        {
            bool isDay = IsDayTime();
            float startDegree = isDay ? 0 : 180;
            
            TimeSpan start = isDay ? _sunriseTime : _sunsetTime;
            TimeSpan end = isDay ? _sunsetTime : _sunriseTime;
            
            TimeSpan totalTime = CalculateDifference(start, end);
            TimeSpan elapsedTime = CalculateDifference(start, _currentTime.TimeOfDay);
            
            double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
            return Mathf.Lerp(startDegree, startDegree + 180, (float) percentage);
        }
        
        public DateTime CurrentTime => _currentTime;
        
        bool IsDayTime() => _currentTime.TimeOfDay > _sunriseTime && _currentTime.TimeOfDay < _sunsetTime;

        TimeSpan CalculateDifference(TimeSpan from, TimeSpan to)
        {
            TimeSpan difference = to - from;
            return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
        }
    }
}