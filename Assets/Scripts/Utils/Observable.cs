using System;

namespace Wattle.Utils
{
    public class Observable<T>
    {
        private T _value;

        public event Action<T> onValueChanged;

        public Observable(T value)
        {
            _value = value;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    onValueChanged?.Invoke(_value);
                }
            }
        }
    }
}