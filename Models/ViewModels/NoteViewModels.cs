using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models.ViewModels
{
    public class NoteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public string FontFamily { get; set; } = "Arial";

        public string TextColor { get; set; } = "#000000";

        public bool IsIncognito { get; set; } = false;

        [StringLength(4, MinimumLength = 4, ErrorMessage = "PIN must be exactly 4 digits")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be 4 digits")]
        public string? PinCode { get; set; }
    }

    public class VerifyPinViewModel
    {
        public int NoteId { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "PIN must be 4 digits")]
        public string PinCode { get; set; } = string.Empty;
    }
}
