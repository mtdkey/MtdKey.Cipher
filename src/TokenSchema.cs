using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtdKey.Cipher
{
    public class TokenSchema
    {
        public string IV { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
    }
}
