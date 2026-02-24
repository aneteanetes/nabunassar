using ioi.Components;

namespace ioi.Entities.Map
{
    internal struct RogueMapCell
    {
        /// <summary>
        /// 1 - Blocked, 2 - Lava, 4 - Water, 8 - Slow, e.t.c.
        /// <para>НЕ УСТАНАВЛИВАЕТСЯ ЧЕРЕЗ ССЫЛКУ</para>
        /// </summary>
        public byte Flags;

        // Прямой доступ к объектам. 
        // В Enterprise мы бы использовали массив, но для гибкости оставим List.
        public List<ObjectMap> Objects;

        public bool IsBlocked => (Flags & 1) != 0;
    }
}
