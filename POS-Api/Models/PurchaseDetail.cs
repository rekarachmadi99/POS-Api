namespace POS_Api.Models
{
    public class PurchaseDetail
    {
        public string PurchaseDetailId { get; set; }
        public string PurchaseId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Product Product { get; set; }
    }
}
