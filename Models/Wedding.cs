using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId { get; set; }
        [Required]
        public string WedderOne { get; set; }
        [Required]
        public string WedderTwo { get; set; }
        

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //-----------------------------------------------
        // public int RSVPId {get;set;} // why no Id here?
        public List<RSVP> Guests {get;set;}

        public int UserId{get;set;}
        public User PlannedBy {get;set;}
    }
}

//one user can build many wedding
//