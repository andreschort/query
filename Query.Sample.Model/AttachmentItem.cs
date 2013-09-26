using System.ComponentModel.DataAnnotations.Schema;

namespace Query.Sample.Model
{
    public class AttachmentItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Deleted { get; set; }

        [NotMapped]
        public AttachmentLocation Location
        {
            get { return (AttachmentLocation) this.Location_Id; }
            set { this.Location_Id = (int) value; }
        }

        public int Location_Id { get; set; }
    }
}