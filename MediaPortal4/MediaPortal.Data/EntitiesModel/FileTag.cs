namespace MediaPortal.Data.EntitiesModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FileTag")]
    public partial class FileTag
    {
        public int Id { get; set; }

        public int FileSystemId { get; set; }

        public int TagId { get; set; }

        public virtual FileSystem FileSystem { get; set; }

        public virtual Tag Tag { get; set; }
    }
}
