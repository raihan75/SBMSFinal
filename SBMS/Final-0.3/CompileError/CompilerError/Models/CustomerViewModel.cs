using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CompileError.Model.Model;
using AutoMapper;
using System.ComponentModel.DataAnnotations;



namespace CompilerError.Models
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Code can not empty")]
        [MaxLength(4, ErrorMessage = "lenght is 4")]
        [MinLength(4, ErrorMessage = "lenght is 4")]

        public string Code { get; set; }

        [Required(ErrorMessage = "Name can not empty")]
        [MaxLength(50, ErrorMessage = "max lenght is 50")]
        [MinLength(3, ErrorMessage = "min lenght is 3")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Name contain character.")]

        public string Name { get; set; }

        [Required(ErrorMessage = "can not empty")]

        public string Address { get; set; }

        [Required(ErrorMessage = "Email can not empty")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "Email is not valid.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "can not empty")]
        [MaxLength(11, ErrorMessage = "lenght is 11")]
        [MinLength(11, ErrorMessage = "lenght is 11")]
        [RegularExpression(@"^[0][1][0-9]+$", ErrorMessage = "contact is not valid.")]


        public string Contact { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = " digit only")]
        [Display(Name = "Loyality Point")]
        public float LoyalityPoint { get; set; } = 10;

        public List<Customer> Customers { get; set; }
    }
}