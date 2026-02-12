using System.ComponentModel.DataAnnotations;

namespace FinanceTrackerAPI.Services.Dto.AI
{
    public class ChatRequestDto
    {
        [Required]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 2000 characters")]
        public string Message { get; set; } = string.Empty;
    }
}
