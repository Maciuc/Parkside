﻿using System.ComponentModel.DataAnnotations;

namespace Parkside.Models.ViewModels
{
    public class PlayerViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Height { get; set; }
        public int? Number { get; set; }
        public string? ImageBase64 { get; set; }
    }

    public class PlayerUpdateViewModel
    {
        [Required(ErrorMessage = "You should provide a firstname value.")]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "You should provide a firstname value.")]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;
        public string? Height { get; set; }
        public int? Number { get; set; }
        public string? ImageBase64 { get; set; }
    }

    public class PlayerCreateViewModel
    {
        [Required(ErrorMessage = "You should provide a firstname value.")]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "You should provide a lastname value.")]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;
        public string? Height { get; set; }
        public int? Number { get; set; }
        public string? ImageBase64 { get; set; }
    }
}
