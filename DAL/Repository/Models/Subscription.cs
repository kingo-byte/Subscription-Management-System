using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; } 

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [ForeignKey("SubscriptionType")]
        public int SubscriptionTypeId { get;set; }

        public virtual User? User { get; set; }

        public virtual SubscriptionType? SubscriptionType { get; set; }
    }
}
