using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Models
{
    public class Session : BaseEntity
    {
        public string Description { get; set; } = default!;

        public int Capacity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        #region Relationships
        public Trainer Trainer { get; set; } = default!;
        public int TrainerId { get; set; }
        #endregion

        #region Relationships
        public Category Category { get; set; } = default!;
        public int CategoryId { get; set; }

        public ICollection<Booking> SessionMembers { get; set; } = default!;
        #endregion
    }
}
