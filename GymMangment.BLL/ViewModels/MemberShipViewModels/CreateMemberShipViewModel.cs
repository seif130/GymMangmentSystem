using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.ViewModels.MemberShipViewModels
{
    public class CreateMemberShipViewModel
    {
        public int PlanId { get; set; }
        public int MemberId { get; set; }
        public DateTime? StartDate { get; set; }

    }
}
