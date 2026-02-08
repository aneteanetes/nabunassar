using Nabunassar.Entities.Struct.FixedCollections.Quads;

namespace Nabunassar.Entities.Struct.FixedCollections.Octas
{
    public class Octa<T> : Quad<T>
    {
        public virtual T Fifth { get; set; }

        public virtual T Sixth { get; set; }

        public OctaPosition GetOctaPosition(T item)
        {
            var quadPos = GetQuadPosition(item);

            if (quadPos != QuadPosition.Unknown)
                return (OctaPosition)(int)quadPos;

            if(item.Equals(Fifth))
                return OctaPosition.Fifth;

            if (item.Equals(Sixth))
                return OctaPosition.Sixth;

            return OctaPosition.Unknown;
        }

        public override IEnumerator<T> GetEnumerator()
        {
            yield return First;
            yield return Second;
            yield return Third;
            yield return Fourth;
            yield return Fifth;
            yield return Sixth;
        }

        public override IEnumerable<T> Reverse()
        {
            yield return Sixth;
            yield return Fifth;
            yield return Fourth;
            yield return Third;
            yield return Second;
            yield return First;
        }

        public override T GetByIndex(int key)
        {
            if (key < 4)
                return base.GetByIndex(key);

            if (key == 4)
                return Fifth;

            if(key == 5)
                return Sixth;

            return default;
        }

        public override void SetByIndex(int key, T value)
        {
            switch (key)
            {
                case 0: First = value; break;
                case 1: Second = value; break;
                case 2: Third = value; break;
                case 3: Fourth = value; break;
                case 4: Fifth = value; break;
                case 5: Sixth = value; break;
                default: throw new InvalidOperationException("Octa support only 0-5 indexes!");
            }
        }

        public T this[OctaPosition octaPosition]
        {
            set
            {
                switch (octaPosition)
                {
                    case OctaPosition.First:
                        First = value;
                        break;
                    case OctaPosition.Second:
                        Second = value;
                        break;
                    case OctaPosition.Third:
                        Third = value;
                        break;
                    case OctaPosition.Fourth:
                        Fourth = value;
                        break;
                    case OctaPosition.Fifth:
                        Fifth = value;
                        break;
                    case OctaPosition.Sixth:
                        Sixth = value;
                        break;
                    default:
                        break;
                }
            }
            get => octaPosition switch
            {
                OctaPosition.First => First,
                OctaPosition.Second => Second,
                OctaPosition.Third => Third,
                OctaPosition.Fourth => Fourth,
                OctaPosition.Fifth => Fifth,
                OctaPosition.Sixth => Sixth,
                _ => First,
            };
        }
    }
}