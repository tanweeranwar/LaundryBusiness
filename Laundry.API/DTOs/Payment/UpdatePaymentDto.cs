namespace Laundry.API.DTOs.Payment;

public class UpdatePaymentDto
{
    public string? Remarks { get; set; }

    public int PaymentStatus { get; set; }
}