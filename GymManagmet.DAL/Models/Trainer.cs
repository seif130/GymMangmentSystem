using GymManagmet.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagmet.DAL.Models
{
    public class Trainer : GymUser
    {
        public Speciality Speciality { get; set; }


        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!;
        #endregion
    }
}
