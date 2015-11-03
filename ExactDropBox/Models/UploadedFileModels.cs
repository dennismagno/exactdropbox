using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExactDropBox.Models
{

    public class UploadedFile
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        public string FileName { get; set; }
        public string DropBoxRev { get; set; }
        public string ExactDocID { get; set; }
        public DateTime DateUploaded { get; set; }
    }

    public class UploadedFileDBContext : DbContext
    {
        public UploadedFileDBContext()
            : base("DefaultConnection") 
        {
            Database.SetInitializer<UploadedFileDBContext>(new CreateDatabaseIfNotExists<UploadedFileDBContext>());
        }
        public DbSet<UploadedFile> UploadedFile { get; set; }
    }
}