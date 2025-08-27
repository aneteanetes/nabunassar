using System.Collections;

namespace Nabunassar.Entities.Struct
{
    public class Quad<T> : IEnumerable<T>
    {
        public virtual T First { get; set; }

        public virtual T Second { get; set; }

        public virtual T Third { get; set; }

        public virtual T Fourth { get; set; }

        public QuadPosition GetPosition(T item)
        {
            if (item.Equals(First))
                return QuadPosition.First;

            if (item.Equals(Second))
                return QuadPosition.Second;

            if (item.Equals(Third))
                return QuadPosition.Third;

            if (item.Equals(Fourth))
                return QuadPosition.Fourth;

            return QuadPosition.First;
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            yield return First;
            yield return Second;
            yield return Third;
            yield return Fourth;
        }

        public virtual IEnumerable<T> Reverse()
        {
            yield return Fourth;
            yield return Third;
            yield return Second;
            yield return First;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public T this[int key]
        {
            get => key switch
            {
                0 => First,
                1 => Second,
                2 => Third,
                3 => Fourth,
                _ => default,
            };

            set
            {
                switch (key)
                {
                    case 0: First = value; break;
                    case 1: Second = value; break;
                    case 2: Third = value; break;
                    case 3: Fourth = value; break;
                    default: throw new InvalidOperationException("Quad support only 0-3 indexes!");
                }
            }
        }

        public T this[QuadPosition quadPosition]
        {
            set
            {
                switch (quadPosition)
                {
                    case QuadPosition.First:
                        First = value;
                        break;
                    case QuadPosition.Second:
                        Second = value;
                        break;
                    case QuadPosition.Third:
                        Third = value;
                        break;
                    case QuadPosition.Fourth:
                        Fourth = value;
                        break;
                    default:
                        break;
                }
            }
            get => quadPosition switch
            {
                QuadPosition.First => First,
                QuadPosition.Second => Second,
                QuadPosition.Third => Third,
                QuadPosition.Fourth => Fourth,
                _ => First,
            };
        }
    }
}