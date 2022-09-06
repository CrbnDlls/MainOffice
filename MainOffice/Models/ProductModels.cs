using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "ProductList", Name = "ProductShortName", ResourceType = typeof(GlobalRes),ControllerName ="Products")]
    public class Product
    {
        [Required]
        [ScaffoldColumn(false)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(200, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProductName", Prompt = "PromptEnter")]
        [Index("IX_ProductUnique",1, IsUnique = true)]
        public string Name { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Index("IX_ProductUnique", 2, IsUnique = true)]
        public int? TrademarkId { get; set; }
        public Trademark Trademark { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Index("IX_ProductUnique", 3, IsUnique = true)]
        public int? ProductlineId { get; set; }
        public Productline Productline { get; set; }
        [Required]
        [AdditionalMetadata("ForeignKey", true)]
        [Index("IX_ProductUnique", 4, IsUnique = true)]
        public int ProductVolumeId { get; set; }
        public ProductVolume ProductVolume { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class ProductJsonViewModel
    {
        public ProductJsonViewModel(Product item)
        {

            id = item.Id;
            Name = item.Name;
            if (item.Trademark != null)
            {
                Trademark = new InternalName() { Name = item.Trademark.Name };
            }
            if (item.Productline != null)
            {
                Productline = new InternalName() { Name = item.Productline.Name };
            }
            if (item.ProductVolume != null)
            {
                ProductVolume = new InternalName() { Name = item.ProductVolume.Name };
            }
        }
        public int id { get; set; }
        public string Name { get; set; }
        public InternalName Trademark { get; set; }
        public InternalName Productline { get; set; }
        public InternalName ProductVolume { get; set; }
        public class InternalName
        {
            public string Name { get; set; }
        }
    }

    public class ProductFilter
    {
        public ProductFilter() { }

        public int[] TrademarkIdSelected { get; set; }


        public int[] ProductlineIdSelected { get; set; }


        public int[] ProductVolumeIdSelected { get; set; }

    }

    [DisplayClass(GroupName = "ProductVolumeList", Name = "ProductVolumeShortName", ResourceType = typeof(GlobalRes), ControllerName ="ProductVolumes")]
    public class ProductVolume
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProductVolumeName", ShortName = "ProductVolumeShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class ProductVolumeJsonViewModel
    {
        public ProductVolumeJsonViewModel(ProductVolume item)
        {

            id = item.Id;
            Name = item.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }

    [DisplayClass(GroupName = "TrademarkList", Name = "TrademarkShortName", ResourceType = typeof(GlobalRes), ControllerName ="Trademarks")]
    public class Trademark
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(50, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "TrademarkName", ShortName = "TrademarkShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class TrademarkJsonViewModel
    {
        public TrademarkJsonViewModel(Trademark item)
        {

            id = item.Id;
            Name = item.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }

    [DisplayClass(GroupName = "ProductlineList", Name = "ProductlineShortName", ResourceType = typeof(GlobalRes), ControllerName ="Productlines")]
    public class Productline
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(200, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ProductlineName", ShortName = "ProductlineShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class ProductlineJsonViewModel
    {
        public ProductlineJsonViewModel(Productline item)
        {
            id = item.Id;
            Name = item.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }
}