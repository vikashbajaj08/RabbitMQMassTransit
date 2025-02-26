namespace OrderService.Dtos
{
    public record OrderRequest
    {
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
    }
}
