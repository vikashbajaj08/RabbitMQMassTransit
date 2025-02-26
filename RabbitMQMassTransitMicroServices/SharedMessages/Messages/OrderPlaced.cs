namespace SharedMessages.Messages
{
    public sealed record OrderPlaced
    {
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
    }
}
