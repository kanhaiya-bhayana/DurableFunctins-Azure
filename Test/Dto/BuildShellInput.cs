using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Dto
{
    public class BuildShellInput
    {
        public ToolsResponse? Tools { get; set; }
        public PartsResponse? Parts { get; set; }
    }
}
