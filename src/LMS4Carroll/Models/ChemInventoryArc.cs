﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LMS4Carroll.Models
{
    public class ChemInventoryArc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Barcode")]
        public int? ChemInventoryIdArc { get; set; }

        [Required]
        [ForeignKey("Order")]
        [Display(Name = "Order ID")]
        public int? OrderID { get; set; }
        public virtual Order Order { get; set; }

        [Required]
        [ForeignKey("Location")]
        public int? LocationID { get; set; }
        public virtual Location Location { get; set; }

        [Required]
        [ForeignKey("Chemical")]
        public int? ChemID { get; set; }
        public virtual Chemical Chemical { get; set; }

        [StringLength(50)]
        [Display(Name = "Location")]
        public string NormalizedLocation { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "CAT Number")]
        public string CAT { get; set; }

        [StringLength(50)]
        [Display(Name = "Lot #")]
        public string LOT { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Units")]
        public string Units { get; set; }

        [Required]
        [Range(0, 999999, ErrorMessage = "Quantity must be between 0 and 999,999")]
        [Display(Name = "Qty Left")]
        public float QtyLeft { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DefaultValue("01/01/1900")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expire Date")]
        public DateTime ExpiryDate { get; set; }

        public virtual ICollection<ChemLog> ChemLogs { get; set; }

    }
}
