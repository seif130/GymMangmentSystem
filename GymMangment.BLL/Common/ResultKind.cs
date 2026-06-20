using System;
using System.Collections.Generic;
using System.Text;

namespace GymMangment.BLL.Common
{
    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden,
    }
}
