namespace NebNes.Mappers.Misc {
    public readonly struct PageTable {
        private readonly byte[] data;
        private readonly int pageSize;

        public PageTable(byte[] data, int pageSize, int length) {
            this.data = data;
            this.pageSize = pageSize;
            Length = length;
        }

        public int Length { get; }

        public Span<byte> this[int page] {
            get { return data.AsSpan(page * pageSize, pageSize); }
        }

        public Span<byte> wrapped(int page) {
            return this[Math.Max(0, page % Length)];
        }
    }
}
