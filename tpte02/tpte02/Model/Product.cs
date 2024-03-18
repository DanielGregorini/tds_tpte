namespace tpte02.Model
{
    public record Product
    {
        public int IdProduto { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
