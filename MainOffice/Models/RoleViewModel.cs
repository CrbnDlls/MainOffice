using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MainOffice.Models
{
    [AdditionalMetadata("List", "Список ролей")]
    [AdditionalMetadata("ShortName", "Роль")]
    public class RoleModel
    {
        public RoleModel() { }

        public RoleModel(ApplicationRole role)
        {
            Id = role.Id;
            Name = role.Name;
        }

        [HiddenInput(DisplayValue = false)]
        public string Id { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Значение {0} должно содержать не менее {2} символов.", MinimumLength = 3)]
        [Display(Name = "Наименование роли")]
        public string Name { get; set; }
       
    }
}