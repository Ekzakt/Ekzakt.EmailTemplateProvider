namespace Ekzakt.EmailTemplateProvider.Core.Models;

public class EmailAddress
{
    public EmailAddress() { }


    public EmailAddress(string address)
    {
        Address = address;
    }

    public EmailAddress(string address, string name)
    {
        Address = address;
        Name = name;
    }


    public string Address { get; init; } = string.Empty;

    public string? Name { get; init; } = string.Empty;

}