﻿using System.ComponentModel.DataAnnotations;

namespace Parkside.Models.ViewModels
{
    public class SocialMediaViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Link { get; set; }
        public string? Platform { get; set; }
    }

    public class SocialMediaUpdateViewModel
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        public string? Link { get; set; }
        public string? Platform { get; set; }
    }

    public class SocialMediaCreateViewModel
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
        public string? Link { get; set; }
        public string? Platform { get; set; }
    }
}