using GlowBook.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlowBook.Wpf.Helpers
{
     public static class AuthSession
    {
        public static ApplicationUser? CurrentUser { get; set; }

        public static void Clear() => CurrentUser = null;
    }
}
