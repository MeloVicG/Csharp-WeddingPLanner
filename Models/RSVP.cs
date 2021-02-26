using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class RSVP
    {
        [Key]
        public int RSVPId { get; set; }
        public int WeddingId { get; set; }
        public int UserId { get; set; } //why are we adding ? for rsvp?
        public User User{get;set;}
        public Wedding Wedding {get;set;}
        // public DateTime CreatedAt { get; set; } = DateTime.Now;
        // public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}