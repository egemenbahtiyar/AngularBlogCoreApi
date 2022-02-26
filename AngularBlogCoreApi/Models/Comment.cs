using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AngularBlogCoreApi.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public int AritcleId { get; set; }
        public string Name { get; set; }
        public string ContentMain { get; set; }
        public DateTime PublishDate { get; set; }

        public virtual Article Aritcle { get; set; }
    }
}
