using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Models
{
    public class Member : GymUser
    {
        public string Photo { get; set; } = default!;


        #region Relationships
        public HealthRecord HealthRecord { get; set; } = default!;

        public ICollection<Membership> Memberships { get; set; } = default!;

        public ICollection<Booking> MemberSessions { get; set; } = default!;
        #endregion


    }
}
