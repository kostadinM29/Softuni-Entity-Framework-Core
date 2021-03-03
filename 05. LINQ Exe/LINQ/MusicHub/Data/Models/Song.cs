﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MusicHub.Data.Models.Enums;

namespace MusicHub.Data.Models
{
    public class Song
    {
        //•	Id – Integer, Primary Key
        //•	Name – Text with max length 20 (required)
        //•	Duration – TimeSpan(required)
        //•	CreatedOn – Date(required)
        //•	Genre ¬– Genre enumeration with possible values: "Blues, Rap, PopMusic, Rock, Jazz" (required)
        //•	AlbumId – Integer, Foreign key
        //•	Album – The song’s album
        //•	WriterId – Integer, Foreign key(required)
        //•	Writer – The song’s writer
        //•	Price – Decimal(required)
        //• SongPerformers – Collection of type SongPerformer

        public Song()
        {
            SongPerformers = new HashSet<SongPerformer>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Album))]
        public int? AlbumId { get; set; } // not req
        public virtual Album Album { get; set; }

        [ForeignKey(nameof(Writer))]
        public int WriterId { get; set; } // req
        public virtual Writer Writer { get; set; }

        public virtual ICollection<SongPerformer> SongPerformers { get; set; }
    }
}
