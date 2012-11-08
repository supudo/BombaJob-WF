using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombaJob.Database.Domain
{
    public class Text
    {
        public virtual Guid ID { get; set; }
        public virtual int TextID { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
    }

    public class Category
    {
        public virtual Guid ID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual int OffersCount { get; set; }
        public virtual string Title { get; set; }
    }

    public class JobOffer
    {
        public virtual Guid ID { get; set; }
        public virtual int OfferID { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual string CategoryTitle { get; set; }
        public virtual string Icon { get; set; }
        public virtual string Title { get; set; }
        public virtual string Email { get; set; }
        public virtual string Negativism { get; set; }
        public virtual string Positivism { get; set; }
        public virtual bool FreelanceYn { get; set; }
        public virtual bool HumanYn { get; set; }
        public virtual bool ReadYn { get; set; }
        public virtual bool SentMessageYn { get; set; }
        public virtual DateTime PublishDate { get; set; }
    }
}
