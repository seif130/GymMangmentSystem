using GymManagmet.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Models
{
    public class Membership : BaseEntity
    {
        public Member Member { get; set; } = default!;

        public int MemberId { get; set; }

        public Plan Plan { get; set; } = default!;
        public int PlanId { get; set; }

        public DateTime EndDate { get; set; }

        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";

        public bool IsActive => EndDate > DateTime.Now;
    }
}
