using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AngularBlogCoreApi.Models
{
    public partial class Article
    {
        public Article()
        {
            Comment = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string ContentSummary { get; set; }
        public string ContentMain { get; set; }
        public DateTime PublishDate { get; set; }
        public string Picture { get; set; }
        public int CategoryId { get; set; }
        public int ViewCount { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
    }
}
