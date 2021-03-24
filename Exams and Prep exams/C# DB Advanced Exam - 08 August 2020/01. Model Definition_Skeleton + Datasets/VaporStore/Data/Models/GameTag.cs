using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VaporStore.Data.Models
{
    public class GameTag
    {
        /*•	GameId – integer, Primary Key, foreign key (required)
•	Game – Game
•	TagId – integer, Primary Key, foreign key (required)
•	Tag – Tag
*/
        //[Required]
        [ForeignKey(nameof(Game))]
        public int GameId { get; set; } 
        public virtual Game Game { get; set; }

        //[Required]
        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
