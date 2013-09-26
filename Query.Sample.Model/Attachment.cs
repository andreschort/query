using System.Collections.Generic;

namespace Query.Sample.Model
{
    public class Attachment
    {
        public int Id { get; set; }

        public List<AttachmentItem> Items { get; set; }

        public bool Deleted { get; set; }
    }
}