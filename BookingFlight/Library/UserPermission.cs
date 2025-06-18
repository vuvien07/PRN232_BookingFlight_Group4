using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class UserPermission<T>
    {
        public string SessionId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public List<T> Permissions { get; set; } = new List<T>();
    }
}
