using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHub_Explorer.NavigationParam
{
    class NavigationParam
    {
    }

    public class RepositoryInfoNaviParam
    {
        public RepositoryInfoNaviParam(string owner, string name)
        {
            this.Owner = owner;
            this.Name = name;
        }
        public string Owner { get; private set; }
        public string Name { get; private set; }

    }
}
