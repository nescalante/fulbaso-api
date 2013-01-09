namespace Fulbaso.Contract
{
    public class Album
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public AlbumType Type { get; set; }

        public int Count { get; set; }

        public long Cover { get; set; }
    }
}
