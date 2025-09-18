namespace ProductGrpc.Server.Data
{
    public class LinkEntities
    {
        public int Id { get; set; }
        public long ParentId { get; set; }
        public ProductEntities? Parent {  get; set; }
        public long ChildId { get; set; }
        public ProductEntities? Child { get; set; }
        public int Quantity { get; set; }
    }
}
