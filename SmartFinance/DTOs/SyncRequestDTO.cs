using System.ComponentModel.DataAnnotations;

namespace SmartFinance.DTOs;

public class SyncRequestDto
{
    [Required]
    public string Symbol { get; set; } = string.Empty;
}