namespace Laundry.API.DTOs.Payment;

public class CreatePaymentDto
{
    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public int PaymentMethod { get; set; }

    public string? TransactionReference { get; set; }

    public string? Remarks { get; set; }

    public string? ReceivedBy { get; set; }
}