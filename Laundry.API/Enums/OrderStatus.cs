namespace Laundry.API.Enums;

public enum OrderStatus
{
    Created = 1,
    Received = 2,
    Washing = 3,
    DryCleaning = 4,
    Ironing = 5,
    QualityCheck = 6,
    Ready = 7,
    OutForDelivery = 8,
    Delivered = 9,
    Cancelled = 10
}