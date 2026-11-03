using System.ComponentModel.DataAnnotations;

namespace InstAlertPlatform.API.Business.Interfaces.Rest.Resources;

public record UpdateBusinessResource(
    [Required(ErrorMessage = "BusinessNameInvalid")]
    [StringLength(150, ErrorMessage = "BusinessNameInvalid")] string Name,
    [Required(ErrorMessage = "BusinessAddressInvalid")]
    [StringLength(250, ErrorMessage = "BusinessAddressInvalid")] string Address,
    [Required(ErrorMessage = "BusinessLatitudeInvalid")]
    [Range(-90d, 90d, ErrorMessage = "BusinessLatitudeInvalid")] double? Latitude,
    [Required(ErrorMessage = "BusinessLongitudeInvalid")]
    [Range(-180d, 180d, ErrorMessage = "BusinessLongitudeInvalid")] double? Longitude,
    [RegularExpression("^[0-9]{11}$", ErrorMessage = "BusinessTaxIdInvalid")] string? TaxId = null);
