namespace POS_Api.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string UserId { get; set; }
        public string CustomerId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Employee Employee { get; set; }
        public Customer Customer { get; set; }
        public ICollection<TransactionDetail> TransactionDetails { get; set; }
    }

}
