using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetEmTrain.Domain.Models
{
    public class Admin : Entity
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
