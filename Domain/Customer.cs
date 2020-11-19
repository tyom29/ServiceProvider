using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;

namespace ServiceProvider.Domain
{
    public class Customer
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string RegCode { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Contact { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Refferal { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Mobile { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.Text)]
        public string Phone { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Column(TypeName = "Date")]
        [DataType(DataType.DateTime)]
        [Required]
        public System.DateTime RegDate { get; set; }

        [Column(TypeName = "bit")]
        public Nullable<bool> AgreementSigned { get; set; }


        [Column(TypeName = "bit")]
        public Nullable<bool> AllowToContact { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        [Required]
        public System.Guid ApiKey { get; set; }


    }

}