namespace POS_Api.Models
{
    public class Purchase
    {
        public string PurchaseId { get; set; }
        public string SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Supplier Supplier { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}
