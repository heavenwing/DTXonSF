using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebFront.Models
{

    public class OrderInput
    {
        public int ProductId { get; set; }

        [Range(1,int.MaxValue)]
        public int Quantity { get; set; }
    }
}