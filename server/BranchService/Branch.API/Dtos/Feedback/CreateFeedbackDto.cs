using System.ComponentModel.DataAnnotations;

namespace Branch.API.Dtos.Feedback
{
    public class CreateFeedbackDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public Guid? UserId { get; set; }

        [Required(ErrorMessage = "OrderId is required.")]
        public Guid? OrderId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        public string Content { get; set; }
    }
}
