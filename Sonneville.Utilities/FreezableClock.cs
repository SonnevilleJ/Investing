using System;

namespace Sonneville.Utilities
{
    public interface IClock
    {
        DateTime Now { get; }
    }

    public class FreezableClock : IClock
    {
        private DateTime? _now;

        public void Freeze(DateTime dateTime)
        {
            Now = dateTime;
        }

        public DateTime Now
        {
            get => _now ?? DateTime.Now;
            private set => _now = value;
        }

        public void Unfreeze()
        {
            _now = null;
        }
    }
}
