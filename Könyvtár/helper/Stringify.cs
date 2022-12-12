using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Könyvtár.Helper
{
    public class Stringify
    {
        public string[] KonyvSTR2 (konyv inp)
        {
            String[] vm = new string[4] {inp.Id.ToString(),inp.ISBN,inp.name,inp.author.ToString()};
            return vm;
        }
    }
}