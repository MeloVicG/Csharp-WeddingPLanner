using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
    //-------------------------------------------------
        [Required(ErrorMessage="Provide First Name")]
        public string FirstName { get; set; }
    //----------------------------------------------------------
        [Required(ErrorMessage="Provide Last Name")]
        public string LastName { get; set; }

    //----------------------------------------------------------
        [EmailAddress]
        [Required(ErrorMessage="Provide Email")]
        public string Email { get; set; }
    //--------------------------------------------------------
        [DataType(DataType.Password)]
        [Required(ErrorMessage="Provide Password")]
        [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
        public string Password { get; set; }
    //----------------------------------------------------------------
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm { get; set; }
        //--------------------------------------------------------------------
        public List<RSVP> PlusOne {get;set;}
        public List<Wedding> PlannedWedding {get;set;}
        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
    }
}