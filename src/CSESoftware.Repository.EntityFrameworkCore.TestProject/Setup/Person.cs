using System.Collections.Generic;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PersonPizza> PersonPizzas { get; set; }
    }
}
