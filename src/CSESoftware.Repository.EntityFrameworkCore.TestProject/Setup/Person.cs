using CSESoftware.Core.Entity;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Person : BaseEntity<int>
    {
        public string Name { get; set; }
        public virtual ICollection<PersonPizza> PersonPizzas { get; set; }
    }
}
