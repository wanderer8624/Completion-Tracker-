using System.ComponentModel.DataAnnotations;
namespace GameVault.ViewModels;
public class RegisterViewModel { [Required, StringLength(40)] public string Name {get;set;}=""; [Required,EmailAddress] public string Email {get;set;}=""; [Required,MinLength(8),DataType(DataType.Password)] public string Password {get;set;}=""; [Required,Compare("Password"),DataType(DataType.Password)] public string ConfirmPassword {get;set;}=""; }
public class LoginViewModel { [Required,EmailAddress] public string Email {get;set;}=""; [Required,DataType(DataType.Password)] public string Password {get;set;}=""; }
