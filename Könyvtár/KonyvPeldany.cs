//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Könyvtár
{
    using System;
    using System.Collections.Generic;
    
    public partial class KonyvPeldany
    {
        public int IdKonvyPeldany { get; set; }
        public string book_id { get; set; }
        public System.DateTime AddedTime { get; set; }
        public Nullable<System.DateTime> RemovedTime { get; set; }
        public int PeldanyId { get; set; }
        public bool isBorrowed { get; set; }
    }
}
