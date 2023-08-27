using System.Security.Cryptography;

namespace ApiDbCache.Db.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public Guid UniqueIdentifier { get; set; }
    public ICollection<Book> Books { get; set; }
}