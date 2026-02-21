namespace TurboSkola.Components.Shared;

public class ProfileDialogResult
{
    public string ProfileName { get; set; } = "";
    public bool IsParent { get; set; }
    public string? PinCode { get; set; }
    public string AvatarColor { get; set; } = "#667eea";
    public string AvatarIcon { get; set; } = "child";
}
