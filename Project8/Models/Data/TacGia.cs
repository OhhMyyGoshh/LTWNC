namespace WebBanSach.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TacGia")]
    public partial class TacGia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TacGia()
        {
            Saches = new HashSet<Sach>();
        }

        [Key]
        [Display(Name = "Mã tác giả")]
        public int MaTG { get; set; }

        [StringLength(50)]
        [Display(Name = "Tên tác giả")]
        [Required(ErrorMessage = "Không được để trống")]
        public string TenTG { get; set; }

        [StringLength(250)]
        [Display(Name = "Quê quán")]
        public string QueQuan { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name = "Ngày mất")]
        public DateTime? NgayMat { get; set; }

        [StringLength(500)]
        [Display(Name = "Tiểu sử")]
        public string TieuSu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sach> Saches { get; set; }
    }
}
