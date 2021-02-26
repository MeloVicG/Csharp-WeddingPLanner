using System;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
            [Key]
            public string Email {get;set;}
            public string Password {get;set;}
    }
}