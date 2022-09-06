using MainOffice.Annotations;
using MainOffice.App_LocalResources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [DisplayClass(GroupName = "ServiceList", Name = "ServiceShortName", ResourceType = typeof(GlobalRes), ControllerName = "Services")]
    public class Service
    {
        [Required]
        [ScaffoldColumn(false)]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(200, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ServiceName", Prompt = "PromptEnter")]
        [Index("IX_ServiceUnique",1, IsUnique = true)]
        public string Name { get; set; }
        [AdditionalMetadata("ForeignKey", true)]
        [Index("IX_ServiceUnique", 2, IsUnique = true)]
        public int? ServiceVolumeId { get; set; }
        public ServiceVolume ServiceVolume { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class ServiceJsonViewModel
    {
        public ServiceJsonViewModel(Service item)
        {

            id = item.Id;
            Name = item.Name;
            if (item.ServiceVolume != null)
            {
                ServiceVolume = new InternalName() { Name = item.ServiceVolume.Name };
            }
        }
        public int id { get; set; }
        public string Name { get; set; }
        public InternalName ServiceVolume { get; set; }
        public class InternalName
        {
            public string Name { get; set; }
        }
    }

    public class ServiceFilter
    {
        public ServiceFilter() { }

        public int[] ServiceVolumeIdSelected { get; set; }

    }

    [DisplayClass(GroupName = "ServiceVolumeList", Name = "ServiceVolumeShortName", ResourceType = typeof(GlobalRes), ControllerName = "ServiceVolumes")]
    public class ServiceVolume
    {
        [Required]
        [ScaffoldColumn(false)]
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "ErrRequired", ErrorMessageResourceType = typeof(GlobalRes))]
        [StringLength(100, ErrorMessageResourceName = "ErrNameLength", ErrorMessageResourceType = typeof(GlobalRes), MinimumLength = 2)]
        [Display(ResourceType = typeof(GlobalRes), Name = "ServiceVolumeName", ShortName = "ServiceVolumeShortName")]
        [DisplayFormat(NullDisplayText = "-")]
        [Index("IX_NameUnique", IsUnique = true)]
        public string Name { get; set; }
        [Timestamp]
        [HiddenInput(DisplayValue = false)]
        public byte[] RowVersion { get; set; }
    }

    public class ServiceVolumeJsonViewModel
    {
        public ServiceVolumeJsonViewModel(ServiceVolume item)
        {

            id = item.Id;
            Name = item.Name;
        }
        public int id { get; set; }
        public string Name { get; set; }
    }

}