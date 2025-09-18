namespace ProductGrpc.Server.Data
{
    public class ProductEntities
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double UnitPrice { get; set; }
        public List<LinkEntities> Children { get; set; } = new();
        public List<LinkEntities> Parents { get; set; } = new();
    }
}
