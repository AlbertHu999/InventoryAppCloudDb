// DTOs/LoginDto.cs（WinForms 專案）
namespace InventoryAppCloudDb.DTOs;

public class LoginDto
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
public class LoginResponseDto
{
    public string Token { get; set; } = "";
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
}